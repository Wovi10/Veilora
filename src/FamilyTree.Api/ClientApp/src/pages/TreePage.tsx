import 'reactflow/dist/style.css';
import { useEffect, useState, useCallback, useRef } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import ReactFlow, { Background, Controls, ConnectionMode, useNodesState } from 'reactflow';
import type { Node, Edge, NodeMouseHandler, Connection, ReactFlowInstance } from 'reactflow';
import {
  Alert,
  Box,
  Button,
  CircularProgress,
  IconButton,
  Typography,
} from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import { getTreeDetails, getTreeRelationships, updatePersonPosition } from '../api/treeDetailApi';
import type { TreeWithPersonsDto, PersonInTreeDto } from '../types/tree';
import type { RelationshipDto } from '../types/relationship';
import type { PersonDto } from '../types/person';
import PersonNode from '../components/PersonNode';
import ParentEdge from '../components/ParentEdge';
import RelationshipEdge from '../components/RelationshipEdge';
import AddPersonDialog from '../components/AddPersonDialog';
import EditPersonDialog from '../components/EditPersonDialog';
import AddRelationshipDialog from '../components/AddRelationshipDialog';

const COLS = 4;
const NODE_W = 180;
const NODE_H = 100;
const GAP = 60;

const nodeTypes = { person: PersonNode };
const edgeTypes = { parentEdge: ParentEdge, relationshipEdge: RelationshipEdge };

function buildNodes(persons: PersonInTreeDto[], onEdit: (person: PersonDto) => void): Node[] {
  return persons.map((pit, i) => ({
    id: pit.person.id,
    type: 'person',
    position: {
      x: pit.positionX ?? (i % COLS) * (NODE_W + GAP),
      y: pit.positionY ?? Math.floor(i / COLS) * (NODE_H + GAP),
    },
    data: { person: pit.person, onEdit },
  }));
}

type Pos = { x: number; y: number };

function pickHandles(p1: Pos, p2: Pos) {
  const dx = p2.x - p1.x;
  const dy = p2.y - p1.y;
  if (Math.abs(dy) >= Math.abs(dx)) {
    return dy >= 0
      ? { sourceHandle: 'bottom', targetHandle: 'top' }
      : { sourceHandle: 'top',    targetHandle: 'bottom' };
  }
  return dx >= 0
    ? { sourceHandle: 'right', targetHandle: 'left' }
    : { sourceHandle: 'left',  targetHandle: 'right' };
}

type ParentBendsFn = (edgeId: string, bends: Record<string, Array<{ x: number; y: number }>>) => void;

function buildParentEdges(persons: PersonDto[], onBendsChange: ParentBendsFn): Edge[] {
  const edges: Edge[] = [];

  // Group children who have both parents by their sorted parent-pair key
  type FamilyGroup = { parent1Id: string; parent2Id: string; childIds: string[] };
  const familyMap = new Map<string, FamilyGroup>();

  for (const person of persons) {
    if (person.parent1Id && person.parent2Id) {
      const [p1, p2] = [person.parent1Id, person.parent2Id].sort();
      const key = `${p1}|${p2}`;
      const existing = familyMap.get(key);
      if (existing) {
        existing.childIds.push(person.id);
      } else {
        familyMap.set(key, { parent1Id: p1, parent2Id: p2, childIds: [person.id] });
      }
    } else {
      const parentId = person.parent1Id ?? person.parent2Id;
      if (parentId) {
        edges.push({
          id: `parent-${person.id}-1`,
          source: parentId,
          target: person.id,
          type: 'parentEdge',
          sourceHandle: 'bottom',
          targetHandle: 'top',
          data: { onBendsChange },
        });
      }
    }
  }

  for (const { parent1Id, parent2Id, childIds } of familyMap.values()) {
    const [primaryChild, ...otherChildren] = childIds;

    // Primary edge: parent1 → firstChild, carries all drawing data
    edges.push({
      id: `parent-${primaryChild}-1`,
      source: parent1Id,
      target: primaryChild,
      type: 'parentEdge',
      sourceHandle: 'bottom',
      targetHandle: 'top',
      data: { siblingParentId: parent2Id, allChildIds: childIds, onBendsChange },
    });

    // Silent: parent2 → firstChild
    edges.push({
      id: `parent-${primaryChild}-2`,
      source: parent2Id,
      target: primaryChild,
      type: 'parentEdge',
      sourceHandle: 'bottom',
      targetHandle: 'top',
      data: { silent: true },
    });

    // Silent: both parents → remaining children
    for (const childId of otherChildren) {
      edges.push({
        id: `parent-${childId}-1`,
        source: parent1Id,
        target: childId,
        type: 'parentEdge',
        sourceHandle: 'bottom',
        targetHandle: 'top',
        data: { silent: true },
      });
      edges.push({
        id: `parent-${childId}-2`,
        source: parent2Id,
        target: childId,
        type: 'parentEdge',
        sourceHandle: 'bottom',
        targetHandle: 'top',
        data: { silent: true },
      });
    }
  }

  return edges;
}

type BendChangeFn = (edgeId: string, bends: Array<{ x: number; y: number }>) => void;

function buildEdges(relationships: RelationshipDto[], posMap: Map<string, Pos>, onBendChange: BendChangeFn): Edge[] {
  return relationships.map((rel) => {
    const p1 = posMap.get(rel.person1Id);
    const p2 = posMap.get(rel.person2Id);
    const handles = p1 && p2 ? pickHandles(p1, p2) : { sourceHandle: 'bottom', targetHandle: 'top' };
    return {
      id: rel.id,
      source: rel.person1Id,
      target: rel.person2Id,
      type: 'relationshipEdge',
      data: { relationshipType: rel.relationshipType, onBendChange },
      ...handles,
    };
  });
}

export default function TreePage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const [tree, setTree] = useState<TreeWithPersonsDto | null>(null);
  const [nodes, setNodes, onNodesChange] = useNodesState([]);
  const [edges, setEdges] = useState<Edge[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [editingPerson, setEditingPerson] = useState<PersonDto | null>(null);
  const rfInstance = useRef<ReactFlowInstance | null>(null);
  const flowContainerRef = useRef<HTMLDivElement | null>(null);
  const [pendingConnection, setPendingConnection] = useState<{
    person1: PersonDto;
    person2: PersonDto;
  } | null>(null);

  const handleEdit = useCallback((person: PersonDto) => {
    setEditingPerson(person);
  }, []);

  const handleEdgeBendChange = useCallback<BendChangeFn>((edgeId, bends) => {
    setEdges(prev => prev.map(e =>
      e.id === edgeId ? { ...e, data: { ...e.data, bends } } : e
    ));
  }, []);

  const handleParentBendsChange = useCallback<ParentBendsFn>((edgeId, bends) => {
    setEdges(prev => prev.map(e =>
      e.id === edgeId ? { ...e, data: { ...e.data, bends } } : e
    ));
  }, []);

  useEffect(() => {
    if (!id) return;
    Promise.all([getTreeDetails(id), getTreeRelationships(id)])
      .then(([treeData, rels]) => {
        setTree(treeData);
        const builtNodes = buildNodes(treeData.persons, handleEdit);
        setNodes(builtNodes);
        const posMap = new Map(builtNodes.map(n => [n.id, n.position]));
        const persons = treeData.persons.map(pit => pit.person);
        setEdges([...buildParentEdges(persons, handleParentBendsChange), ...buildEdges(rels, posMap, handleEdgeBendChange)]);
      })
      .catch((err: unknown) =>
        setError(err instanceof Error ? err.message : 'Unexpected error')
      )
      .finally(() => setLoading(false));
  }, [id, handleEdit, handleEdgeBendChange, handleParentBendsChange]);

  const handlePersonCreated = useCallback((person: PersonDto) => {
    setTree((prev) => {
      if (!prev) return prev;
      return { ...prev, persons: [...prev.persons, { person, positionX: null, positionY: null }] };
    });
    setNodes((prev) => {
      const vp = rfInstance.current?.getViewport() ?? { x: 0, y: 0, zoom: 1 };
      const rect = flowContainerRef.current?.getBoundingClientRect();
      const cx = rect ? rect.width / 2 : 400;
      const cy = rect ? rect.height / 2 : 300;
      const flowX = (-vp.x + cx) / vp.zoom - NODE_W / 2;
      const flowY = (-vp.y + cy) / vp.zoom - NODE_H / 2;
      return [...prev, {
        id: person.id,
        type: 'person',
        position: { x: flowX, y: flowY },
        data: { person, onEdit: handleEdit, isNew: true },
      }];
    });
  }, [handleEdit]);

  const handleNodeDragStop = useCallback<NodeMouseHandler>((_event, node) => {
    if (!id) return;
    updatePersonPosition(id, node.id, node.position.x, node.position.y);
    if (node.data.isNew) {
      setNodes(prev => prev.map(n =>
        n.id === node.id ? { ...n, data: { ...n.data, isNew: false } } : n
      ));
    }
  }, [id]);

  const handleConnect = useCallback((connection: Connection) => {
    if (!connection.source || !connection.target) return;
    const person1 = tree?.persons.find(p => p.person.id === connection.source)?.person;
    const person2 = tree?.persons.find(p => p.person.id === connection.target)?.person;
    if (!person1 || !person2) return;
    setPendingConnection({ person1, person2 });
  }, [tree]);

  const handleRedrawLines = useCallback(() => {
    if (!tree) return;
    const persons = tree.persons.map(pit => pit.person);
    setEdges(prev => {
      const nonParentEdges = prev.filter(e => !e.id.startsWith('parent-'));
      return [...nonParentEdges, ...buildParentEdges(persons, handleParentBendsChange)];
    });
  }, [tree, handleParentBendsChange]);

  const handleRelationshipCreated = useCallback((rel: RelationshipDto) => {
    const n1 = nodes.find(n => n.id === rel.person1Id);
    const n2 = nodes.find(n => n.id === rel.person2Id);
    const handles = n1 && n2 ? pickHandles(n1.position, n2.position) : { sourceHandle: 'bottom', targetHandle: 'top' };
    setEdges(prev => [...prev, {
      id: rel.id,
      source: rel.person1Id,
      target: rel.person2Id,
      type: 'relationshipEdge',
      data: { relationshipType: rel.relationshipType, onBendChange: handleEdgeBendChange },
      ...handles,
    }]);
    setPendingConnection(null);
  }, [nodes, handleEdgeBendChange]);

  const handlePersonSaved = useCallback((updated: PersonDto) => {
    const updatedPersons = (tree?.persons ?? []).map(p =>
      p.person.id === updated.id ? updated : p.person
    );
    setTree((prev) => {
      if (!prev) return prev;
      return { ...prev, persons: prev.persons.map((p) => p.person.id === updated.id ? { ...p, person: updated } : p) };
    });
    setNodes((prev) =>
      prev.map((n) =>
        n.id === updated.id ? { ...n, data: { ...n.data, person: updated } } : n
      )
    );
    setEdges((prev) => {
      const nonParentEdges = prev.filter(e => !e.id.startsWith('parent-'));
      return [...nonParentEdges, ...buildParentEdges(updatedPersons, handleParentBendsChange)];
    });
    setEditingPerson(null);
  }, [tree, handleParentBendsChange]);

  return (
    <>
      <Box
        sx={{
          height: 64,
          display: 'flex',
          alignItems: 'center',
          px: 2,
          gap: 1,
          borderBottom: '1px solid',
          borderColor: 'divider',
        }}
      >
        <IconButton onClick={() => navigate('/')} size="small">
          <ArrowBackIcon />
        </IconButton>
        <Typography variant="h6" sx={{ flexGrow: 1 }}>
          {tree?.name ?? '…'}
        </Typography>
        <Button
          variant="outlined"
          size="small"
          onClick={handleRedrawLines}
          disabled={loading || !!error}
        >
          Redraw Lines
        </Button>
        <Button
          variant="contained"
          size="small"
          onClick={() => setDialogOpen(true)}
          disabled={loading || !!error}
        >
          Add Person
        </Button>
      </Box>

      {loading && (
        <Box sx={{ display: 'flex', justifyContent: 'center', mt: 8 }}>
          <CircularProgress />
        </Box>
      )}

      {error && (
        <Alert severity="error" sx={{ m: 2 }}>
          {error}
        </Alert>
      )}

      {!loading && !error && (
        <>
          {tree && tree.persons.length === 0 && (
            <Box sx={{ textAlign: 'center', mt: 8 }}>
              <Typography color="text.secondary" gutterBottom>
                No people in this tree yet.
              </Typography>
              <Button variant="outlined" onClick={() => setDialogOpen(true)}>
                Add Person
              </Button>
            </Box>
          )}

          {tree && tree.persons.length > 0 && (
            <Box ref={flowContainerRef} sx={{ height: 'calc(100vh - 128px)' }}>
              <ReactFlow
                nodes={nodes}
                edges={edges}
                nodeTypes={nodeTypes}
                edgeTypes={edgeTypes}
                onNodesChange={onNodesChange}
                onNodeDragStop={handleNodeDragStop}
                onConnect={handleConnect}
                onInit={(instance) => { rfInstance.current = instance; }}
                connectionMode={ConnectionMode.Loose}
                fitView
              >
                <Background />
                <Controls />
              </ReactFlow>
            </Box>
          )}
        </>
      )}

      {id && (
        <AddPersonDialog
          open={dialogOpen}
          treeId={id}
          onClose={() => setDialogOpen(false)}
          onCreated={handlePersonCreated}
        />
      )}

      {editingPerson && (
        <EditPersonDialog
          open
          person={editingPerson}
          persons={tree?.persons.map(pit => pit.person) ?? []}
          onClose={() => setEditingPerson(null)}
          onSaved={handlePersonSaved}
        />
      )}

      {pendingConnection && (
        <AddRelationshipDialog
          open
          person1={pendingConnection.person1}
          person2={pendingConnection.person2}
          onClose={() => setPendingConnection(null)}
          onCreated={handleRelationshipCreated}
        />
      )}
    </>
  );
}
