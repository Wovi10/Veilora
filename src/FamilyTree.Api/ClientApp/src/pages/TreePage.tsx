import 'reactflow/dist/style.css';
import { useEffect, useState, useCallback } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import ReactFlow, { Background, Controls, ConnectionMode, useNodesState } from 'reactflow';
import type { Node, Edge, NodeMouseHandler, Connection } from 'reactflow';
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
import AddPersonDialog from '../components/AddPersonDialog';
import EditPersonDialog from '../components/EditPersonDialog';
import AddRelationshipDialog from '../components/AddRelationshipDialog';

const COLS = 4;
const NODE_W = 180;
const NODE_H = 100;
const GAP = 60;

const nodeTypes = { person: PersonNode };
const edgeTypes = { parentEdge: ParentEdge };

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

function buildParentEdge(personId: string, parentId: string, slot: 1 | 2, siblingParentId?: string): Edge {
  return {
    id: `parent-${personId}-${slot}`,
    source: parentId,
    target: personId,
    type: 'parentEdge',
    sourceHandle: 'bottom',
    targetHandle: 'top',
    data: siblingParentId ? { siblingParentId } : {},
  };
}

function buildParentEdges(persons: PersonDto[]): Edge[] {
  const edges: Edge[] = [];
  for (const person of persons) {
    if (person.parent1Id)
      edges.push(buildParentEdge(person.id, person.parent1Id, 1, person.parent2Id ?? undefined));
    if (person.parent2Id)
      edges.push(buildParentEdge(person.id, person.parent2Id, 2, person.parent1Id ?? undefined));
  }
  return edges;
}

function buildEdges(relationships: RelationshipDto[], posMap: Map<string, Pos>): Edge[] {
  return relationships.map((rel) => {
    const p1 = posMap.get(rel.person1Id);
    const p2 = posMap.get(rel.person2Id);
    const handles = p1 && p2 ? pickHandles(p1, p2) : { sourceHandle: 'bottom', targetHandle: 'top' };
    return {
      id: rel.id,
      source: rel.person1Id,
      target: rel.person2Id,
      type: 'smoothstep',
      label: rel.relationshipType,
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
  const [pendingConnection, setPendingConnection] = useState<{
    person1: PersonDto;
    person2: PersonDto;
  } | null>(null);

  const handleEdit = useCallback((person: PersonDto) => {
    setEditingPerson(person);
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
        setEdges([...buildParentEdges(persons), ...buildEdges(rels, posMap)]);
      })
      .catch((err: unknown) =>
        setError(err instanceof Error ? err.message : 'Unexpected error')
      )
      .finally(() => setLoading(false));
  }, [id, handleEdit]);

  const handlePersonCreated = useCallback((person: PersonDto) => {
    setTree((prev) => {
      if (!prev) return prev;
      return { ...prev, persons: [...prev.persons, { person, positionX: null, positionY: null }] };
    });
    setNodes((prev) => {
      const next = [...prev, {
        id: person.id,
        type: 'person',
        position: {
          x: (prev.length % COLS) * (NODE_W + GAP),
          y: Math.floor(prev.length / COLS) * (NODE_H + GAP),
        },
        data: { person, onEdit: handleEdit },
      }];
      return next;
    });
  }, [handleEdit]);

  const handleNodeDragStop = useCallback<NodeMouseHandler>((_event, node) => {
    if (!id) return;
    updatePersonPosition(id, node.id, node.position.x, node.position.y);
  }, [id]);

  const handleConnect = useCallback((connection: Connection) => {
    if (!connection.source || !connection.target) return;
    const person1 = tree?.persons.find(p => p.person.id === connection.source)?.person;
    const person2 = tree?.persons.find(p => p.person.id === connection.target)?.person;
    if (!person1 || !person2) return;
    setPendingConnection({ person1, person2 });
  }, [tree]);

  const handleRelationshipCreated = useCallback((rel: RelationshipDto) => {
    const n1 = nodes.find(n => n.id === rel.person1Id);
    const n2 = nodes.find(n => n.id === rel.person2Id);
    const handles = n1 && n2 ? pickHandles(n1.position, n2.position) : { sourceHandle: 'bottom', targetHandle: 'top' };
    setEdges(prev => [...prev, {
      id: rel.id,
      source: rel.person1Id,
      target: rel.person2Id,
      type: 'smoothstep',
      label: rel.relationshipType,
      ...handles,
    }]);
    setPendingConnection(null);
  }, [nodes]);

  const handlePersonSaved = useCallback((updated: PersonDto) => {
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
      const withoutOld = prev.filter(
        e => e.id !== `parent-${updated.id}-1` && e.id !== `parent-${updated.id}-2`
      );
      const newParentEdges: Edge[] = [];
      if (updated.parent1Id)
        newParentEdges.push(buildParentEdge(updated.id, updated.parent1Id, 1, updated.parent2Id ?? undefined));
      if (updated.parent2Id)
        newParentEdges.push(buildParentEdge(updated.id, updated.parent2Id, 2, updated.parent1Id ?? undefined));
      return [...withoutOld, ...newParentEdges];
    });
    setEditingPerson(null);
  }, []);

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
            <Box sx={{ height: 'calc(100vh - 128px)' }}>
              <ReactFlow
                nodes={nodes}
                edges={edges}
                nodeTypes={nodeTypes}
                edgeTypes={edgeTypes}
                onNodesChange={onNodesChange}
                onNodeDragStop={handleNodeDragStop}
                onConnect={handleConnect}
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
