import 'reactflow/dist/style.css';
import { useEffect, useState, useCallback, useRef } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import ReactFlow, { Background, Controls, ConnectionMode, useNodesState } from 'reactflow';
import type { Node, Edge, NodeMouseHandler, Connection, ReactFlowInstance } from 'reactflow';
import {
  Alert, Box, Button, CircularProgress, IconButton, Typography,
} from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import { getFamilyTreeWithEntities, getFamilyTreeRelationships, updateEntityPosition } from '../api/familyTreesApi';
import type { FamilyTreeWithEntitiesDto, EntityInFamilyTreeDto } from '../types/familyTree';
import type { RelationshipDto } from '../types/relationship';
import type { EntityDto } from '../types/entity';
import EntityNode from '../components/EntityNode';
import ParentEdge from '../components/ParentEdge';
import RelationshipEdge from '../components/RelationshipEdge';
import AddEntityToTreeDialog from '../components/AddEntityToTreeDialog';
import EditEntityDialog from '../components/EditEntityDialog';
import AddRelationshipDialog from '../components/AddRelationshipDialog';

const COLS = 4;
const NODE_W = 180;
const NODE_H = 100;
const GAP = 60;

const nodeTypes = { entity: EntityNode };
const edgeTypes = { parentEdge: ParentEdge, relationshipEdge: RelationshipEdge };

function buildNodes(entries: EntityInFamilyTreeDto[], onEdit: (entity: EntityDto) => void): Node[] {
  return entries.map((entry, i) => ({
    id: entry.entity.id,
    type: 'entity',
    position: {
      x: entry.positionX ?? (i % COLS) * (NODE_W + GAP),
      y: entry.positionY ?? Math.floor(i / COLS) * (NODE_H + GAP),
    },
    data: { entity: entry.entity, onEdit },
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

function buildParentEdges(entities: EntityDto[], onBendsChange: ParentBendsFn): Edge[] {
  const edges: Edge[] = [];

  type FamilyGroup = { parent1Id: string; parent2Id: string; childIds: string[] };
  const familyMap = new Map<string, FamilyGroup>();

  for (const entity of entities) {
    if (entity.parent1Id && entity.parent2Id) {
      const [p1, p2] = [entity.parent1Id, entity.parent2Id].sort();
      const key = `${p1}|${p2}`;
      const existing = familyMap.get(key);
      if (existing) {
        existing.childIds.push(entity.id);
      } else {
        familyMap.set(key, { parent1Id: p1, parent2Id: p2, childIds: [entity.id] });
      }
    } else {
      const parentId = entity.parent1Id ?? entity.parent2Id;
      if (parentId) {
        edges.push({
          id: `parent-${entity.id}-1`,
          source: parentId,
          target: entity.id,
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

    edges.push({
      id: `parent-${primaryChild}-1`,
      source: parent1Id,
      target: primaryChild,
      type: 'parentEdge',
      sourceHandle: 'bottom',
      targetHandle: 'top',
      data: { siblingParentId: parent2Id, allChildIds: childIds, onBendsChange },
    });
    edges.push({
      id: `parent-${primaryChild}-2`,
      source: parent2Id,
      target: primaryChild,
      type: 'parentEdge',
      sourceHandle: 'bottom',
      targetHandle: 'top',
      data: { silent: true },
    });

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

function buildRelationshipEdges(relationships: RelationshipDto[], posMap: Map<string, Pos>, onBendChange: BendChangeFn): Edge[] {
  return relationships.map(rel => {
    const p1 = posMap.get(rel.entity1Id);
    const p2 = posMap.get(rel.entity2Id);
    const handles = p1 && p2 ? pickHandles(p1, p2) : { sourceHandle: 'bottom', targetHandle: 'top' };
    return {
      id: rel.id,
      source: rel.entity1Id,
      target: rel.entity2Id,
      type: 'relationshipEdge',
      data: { relationshipType: rel.relationshipType, onBendChange },
      ...handles,
    };
  });
}

export default function FamilyTreePage() {
  const { familyTreeId } = useParams<{ familyTreeId: string }>();
  const navigate = useNavigate();

  const [tree, setTree] = useState<FamilyTreeWithEntitiesDto | null>(null);
  const [nodes, setNodes, onNodesChange] = useNodesState([]);
  const [edges, setEdges] = useState<Edge[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [addToTreeOpen, setAddToTreeOpen] = useState(false);
  const [editingEntity, setEditingEntity] = useState<EntityDto | null>(null);
  const rfInstance = useRef<ReactFlowInstance | null>(null);
  const flowContainerRef = useRef<HTMLDivElement | null>(null);
  const [pendingConnection, setPendingConnection] = useState<{
    entity1: EntityDto;
    entity2: EntityDto;
  } | null>(null);

  const handleEdit = useCallback((entity: EntityDto) => {
    setEditingEntity(entity);
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
    if (!familyTreeId) return;
    Promise.all([getFamilyTreeWithEntities(familyTreeId), getFamilyTreeRelationships(familyTreeId)])
      .then(([treeData, rels]) => {
        setTree(treeData);
        const builtNodes = buildNodes(treeData.entities, handleEdit);
        setNodes(builtNodes);
        const posMap = new Map(builtNodes.map(n => [n.id, n.position]));
        const entities = treeData.entities.map(e => e.entity);
        setEdges([
          ...buildParentEdges(entities, handleParentBendsChange),
          ...buildRelationshipEdges(rels, posMap, handleEdgeBendChange),
        ]);
      })
      .catch((err: unknown) => setError(err instanceof Error ? err.message : 'Unexpected error'))
      .finally(() => setLoading(false));
  }, [familyTreeId, handleEdit, handleEdgeBendChange, handleParentBendsChange]);

  const handleEntityAddedToTree = useCallback((entity: EntityDto) => {
    setTree(prev => {
      if (!prev) return prev;
      return { ...prev, entities: [...prev.entities, { entity, positionX: null, positionY: null }] };
    });
    setNodes(prev => {
      const vp = rfInstance.current?.getViewport() ?? { x: 0, y: 0, zoom: 1 };
      const rect = flowContainerRef.current?.getBoundingClientRect();
      const cx = rect ? rect.width / 2 : 400;
      const cy = rect ? rect.height / 2 : 300;
      const flowX = (-vp.x + cx) / vp.zoom - NODE_W / 2;
      const flowY = (-vp.y + cy) / vp.zoom - NODE_H / 2;
      return [...prev, {
        id: entity.id,
        type: 'entity',
        position: { x: flowX, y: flowY },
        data: { entity, onEdit: handleEdit, isNew: true },
      }];
    });
  }, [handleEdit]);

  const handleNodeDragStop = useCallback<NodeMouseHandler>((_event, node) => {
    if (!familyTreeId) return;
    updateEntityPosition(familyTreeId, node.id, { x: node.position.x, y: node.position.y });
    if (node.data.isNew) {
      setNodes(prev => prev.map(n =>
        n.id === node.id ? { ...n, data: { ...n.data, isNew: false } } : n
      ));
    }
  }, [familyTreeId]);

  const handleConnect = useCallback((connection: Connection) => {
    if (!connection.source || !connection.target) return;
    const entity1 = tree?.entities.find(e => e.entity.id === connection.source)?.entity;
    const entity2 = tree?.entities.find(e => e.entity.id === connection.target)?.entity;
    if (!entity1 || !entity2) return;
    setPendingConnection({ entity1, entity2 });
  }, [tree]);

  const handleRedrawLines = useCallback(() => {
    if (!tree) return;
    const entities = tree.entities.map(e => e.entity);
    setEdges(prev => {
      const nonParentEdges = prev.filter(e => !e.id.startsWith('parent-'));
      return [...nonParentEdges, ...buildParentEdges(entities, handleParentBendsChange)];
    });
  }, [tree, handleParentBendsChange]);

  const handleRelationshipCreated = useCallback((rel: RelationshipDto) => {
    const n1 = nodes.find(n => n.id === rel.entity1Id);
    const n2 = nodes.find(n => n.id === rel.entity2Id);
    const handles = n1 && n2 ? pickHandles(n1.position, n2.position) : { sourceHandle: 'bottom', targetHandle: 'top' };
    setEdges(prev => [...prev, {
      id: rel.id,
      source: rel.entity1Id,
      target: rel.entity2Id,
      type: 'relationshipEdge',
      data: { relationshipType: rel.relationshipType, onBendChange: handleEdgeBendChange },
      ...handles,
    }]);
    setPendingConnection(null);
  }, [nodes, handleEdgeBendChange]);

  const handleEntitySaved = useCallback((updated: EntityDto) => {
    const updatedEntities = (tree?.entities ?? []).map(e =>
      e.entity.id === updated.id ? updated : e.entity
    );
    setTree(prev => {
      if (!prev) return prev;
      return { ...prev, entities: prev.entities.map(e => e.entity.id === updated.id ? { ...e, entity: updated } : e) };
    });
    setNodes(prev => prev.map(n =>
      n.id === updated.id ? { ...n, data: { ...n.data, entity: updated } } : n
    ));
    setEdges(prev => {
      const nonParentEdges = prev.filter(e => !e.id.startsWith('parent-'));
      return [...nonParentEdges, ...buildParentEdges(updatedEntities, handleParentBendsChange)];
    });
    setEditingEntity(null);
  }, [tree, handleParentBendsChange]);

  const backPath = tree ? `/worlds/${tree.worldId}` : '/';

  return (
    <>
      <Box sx={{ height: 64, display: 'flex', alignItems: 'center', px: 2, gap: 1, borderBottom: '1px solid', borderColor: 'divider' }}>
        <IconButton onClick={() => navigate(backPath)} size="small">
          <ArrowBackIcon />
        </IconButton>
        <Typography variant="h6" sx={{ flexGrow: 1 }}>
          {tree?.name ?? '…'}
        </Typography>
        <Button variant="outlined" size="small" onClick={handleRedrawLines} disabled={loading || !!error}>
          Redraw Lines
        </Button>
        <Button variant="contained" size="small" onClick={() => setAddToTreeOpen(true)} disabled={loading || !!error}>
          Add to Tree
        </Button>
      </Box>

      {loading && <Box display="flex" justifyContent="center" mt={8}><CircularProgress /></Box>}
      {error && <Alert severity="error" sx={{ m: 2 }}>{error}</Alert>}

      {!loading && !error && (
        <>
          {tree && tree.entities.length === 0 && (
            <Box textAlign="center" mt={8}>
              <Typography color="text.secondary" gutterBottom>No entities in this tree yet.</Typography>
              <Button variant="outlined" onClick={() => setAddToTreeOpen(true)}>Add to Tree</Button>
            </Box>
          )}

          {tree && tree.entities.length > 0 && (
            <Box ref={flowContainerRef} sx={{ height: 'calc(100vh - 128px)' }}>
              <ReactFlow
                nodes={nodes}
                edges={edges}
                nodeTypes={nodeTypes}
                edgeTypes={edgeTypes}
                onNodesChange={onNodesChange}
                onNodeDragStop={handleNodeDragStop}
                onConnect={handleConnect}
                onInit={instance => { rfInstance.current = instance; }}
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

      {familyTreeId && tree && (
        <AddEntityToTreeDialog
          open={addToTreeOpen}
          familyTreeId={familyTreeId}
          worldId={tree.worldId}
          existingEntityIds={tree.entities.map(e => e.entity.id)}
          onClose={() => setAddToTreeOpen(false)}
          onAdded={handleEntityAddedToTree}
        />
      )}

      {editingEntity && (
        <EditEntityDialog
          open
          entity={editingEntity}
          treeEntities={tree?.entities.map(e => e.entity) ?? []}
          onClose={() => setEditingEntity(null)}
          onSaved={handleEntitySaved}
        />
      )}

      {pendingConnection && (
        <AddRelationshipDialog
          open
          entity1={pendingConnection.entity1}
          entity2={pendingConnection.entity2}
          onClose={() => setPendingConnection(null)}
          onCreated={handleRelationshipCreated}
        />
      )}
    </>
  );
}
