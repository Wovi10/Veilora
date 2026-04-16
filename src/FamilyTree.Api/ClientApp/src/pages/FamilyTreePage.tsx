import 'reactflow/dist/style.css';
import { useEffect, useState, useCallback, useRef } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import ReactFlow, { Background, Controls, ConnectionMode, useNodesState } from 'reactflow';
import type { Node, Edge, NodeMouseHandler, Connection, ReactFlowInstance } from 'reactflow';
import {
  Alert, Box, Button, CircularProgress, IconButton, Typography,
} from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import { getFamilyTreeWithEntities, getFamilyTreeRelationships, updateCharacterPosition } from '../api/familyTreesApi';
import { getCharactersByWorld } from '../api/charactersApi';
import { getEntities } from '../api/entitiesApi';
import type { FamilyTreeWithEntitiesDto } from '../types/familyTree';
import type { RelationshipDto } from '../types/relationship';
import type { CharacterDto } from '../types/character';
import type { EntityDto } from '../types/entity';
import CharacterNode from '../components/CharacterNode';
import ParentEdge from '../components/ParentEdge';
import RelationshipEdge from '../components/RelationshipEdge';
import AddEntityToTreeDialog from '../components/AddEntityToTreeDialog';
import EditCharacterDialog from '../components/EditCharacterDialog';
import AddRelationshipDialog from '../components/AddRelationshipDialog';

const COLS = 4;
const NODE_W = 180;
const NODE_H = 100;
const GAP = 60;

const nodeTypes = { character: CharacterNode };
const edgeTypes = { parentEdge: ParentEdge, relationshipEdge: RelationshipEdge };

function buildNodes(
  entries: FamilyTreeWithEntitiesDto['characters'],
  onEdit: (character: CharacterDto) => void,
): Node[] {
  return entries.map((entry, i) => ({
    id: entry.character.id,
    type: 'character',
    position: {
      x: entry.positionX ?? (i % COLS) * (NODE_W + GAP),
      y: entry.positionY ?? Math.floor(i / COLS) * (NODE_H + GAP),
    },
    data: { character: entry.character, onEdit },
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

function buildParentEdges(characters: CharacterDto[], onBendsChange: ParentBendsFn): Edge[] {
  const edges: Edge[] = [];

  type FamilyGroup = { parent1Id: string; parent2Id: string; childIds: string[] };
  const familyMap = new Map<string, FamilyGroup>();

  for (const character of characters) {
    if (character.parent1Id && character.parent2Id) {
      const [p1, p2] = [character.parent1Id, character.parent2Id].sort();
      const key = `${p1}|${p2}`;
      const existing = familyMap.get(key);
      if (existing) {
        existing.childIds.push(character.id);
      } else {
        familyMap.set(key, { parent1Id: p1, parent2Id: p2, childIds: [character.id] });
      }
    } else {
      const parentId = character.parent1Id ?? character.parent2Id;
      if (parentId) {
        edges.push({
          id: `parent-${character.id}-1`,
          source: parentId,
          target: character.id,
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
    const p1 = posMap.get(rel.character1Id);
    const p2 = posMap.get(rel.character2Id);
    const handles = p1 && p2 ? pickHandles(p1, p2) : { sourceHandle: 'bottom', targetHandle: 'top' };
    return {
      id: rel.id,
      source: rel.character1Id,
      target: rel.character2Id,
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
  const [worldCharacters, setWorldCharacters] = useState<CharacterDto[]>([]);
  const [worldEntities, setWorldEntities] = useState<EntityDto[]>([]);
  const [nodes, setNodes, onNodesChange] = useNodesState([]);
  const [edges, setEdges] = useState<Edge[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [addToTreeOpen, setAddToTreeOpen] = useState(false);
  const [editingCharacter, setEditingCharacter] = useState<CharacterDto | null>(null);
  const rfInstance = useRef<ReactFlowInstance | null>(null);
  const flowContainerRef = useRef<HTMLDivElement | null>(null);
  const [pendingConnection, setPendingConnection] = useState<{
    character1: CharacterDto;
    character2: CharacterDto;
  } | null>(null);

  const handleEdit = useCallback((character: CharacterDto) => {
    setEditingCharacter(character);
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
    (async () => {
      try {
        const [treeData, rels] = await Promise.all([
          getFamilyTreeWithEntities(familyTreeId),
          getFamilyTreeRelationships(familyTreeId).catch(() => [] as RelationshipDto[]),
        ]);
        setTree(treeData);

        const [worldChars, allEntities] = await Promise.all([
          getCharactersByWorld(treeData.worldId).catch(() => [] as CharacterDto[]),
          getEntities().catch(() => []),
        ]);
        setWorldCharacters(worldChars);
        setWorldEntities(allEntities.filter(e => e.worldId === treeData.worldId));

        const builtNodes = buildNodes(treeData.characters, handleEdit);
        setNodes(builtNodes);
        const posMap = new Map(builtNodes.map(n => [n.id, n.position]));
        const chars = treeData.characters.map(e => e.character);
        setEdges([
          ...buildParentEdges(chars, handleParentBendsChange),
          ...buildRelationshipEdges(rels, posMap, handleEdgeBendChange),
        ]);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Unexpected error');
      } finally {
        setLoading(false);
      }
    })();
  }, [familyTreeId, handleEdit, handleEdgeBendChange, handleParentBendsChange]);

  const handleCharacterAddedToTree = useCallback((character: CharacterDto) => {
    setTree(prev => {
      if (!prev) return prev;
      return { ...prev, characters: [...prev.characters, { character, positionX: null, positionY: null }] };
    });
    setNodes(prev => {
      const vp = rfInstance.current?.getViewport() ?? { x: 0, y: 0, zoom: 1 };
      const rect = flowContainerRef.current?.getBoundingClientRect();
      const cx = rect ? rect.width / 2 : 400;
      const cy = rect ? rect.height / 2 : 300;
      const flowX = (-vp.x + cx) / vp.zoom - NODE_W / 2;
      const flowY = (-vp.y + cy) / vp.zoom - NODE_H / 2;
      return [...prev, {
        id: character.id,
        type: 'character',
        position: { x: flowX, y: flowY },
        data: { character, onEdit: handleEdit, isNew: true },
      }];
    });
    setWorldCharacters(prev => [...prev, character]);
  }, [handleEdit]);

  const handleCharacterDeleted = useCallback(() => {
    if (!editingCharacter) return;
    const id = editingCharacter.id;
    setTree(prev => prev ? { ...prev, characters: prev.characters.filter(e => e.character.id !== id) } : prev);
    setNodes(prev => prev.filter(n => n.id !== id));
    setEdges(prev => prev.filter(e => e.source !== id && e.target !== id));
    setEditingCharacter(null);
  }, [editingCharacter]);

  const handleNodeDragStop = useCallback<NodeMouseHandler>((_event, node) => {
    if (!familyTreeId) return;
    updateCharacterPosition(familyTreeId, node.id, { x: node.position.x, y: node.position.y });
    if (node.data.isNew) {
      setNodes(prev => prev.map(n =>
        n.id === node.id ? { ...n, data: { ...n.data, isNew: false } } : n
      ));
    }
  }, [familyTreeId]);

  const handleConnect = useCallback((connection: Connection) => {
    if (!connection.source || !connection.target) return;
    const char1 = tree?.characters.find(e => e.character.id === connection.source)?.character;
    const char2 = tree?.characters.find(e => e.character.id === connection.target)?.character;
    if (!char1 || !char2) return;
    setPendingConnection({ character1: char1, character2: char2 });
  }, [tree]);

  const handleRedrawLines = useCallback(() => {
    if (!tree) return;
    const chars = tree.characters.map(e => e.character);
    setEdges(prev => {
      const nonParentEdges = prev.filter(e => !e.id.startsWith('parent-'));
      return [...nonParentEdges, ...buildParentEdges(chars, handleParentBendsChange)];
    });
  }, [tree, handleParentBendsChange]);

  const handleRelationshipCreated = useCallback((rel: RelationshipDto) => {
    const n1 = nodes.find(n => n.id === rel.character1Id);
    const n2 = nodes.find(n => n.id === rel.character2Id);
    const handles = n1 && n2 ? pickHandles(n1.position, n2.position) : { sourceHandle: 'bottom', targetHandle: 'top' };
    setEdges(prev => [...prev, {
      id: rel.id,
      source: rel.character1Id,
      target: rel.character2Id,
      type: 'relationshipEdge',
      data: { relationshipType: rel.relationshipType, onBendChange: handleEdgeBendChange },
      ...handles,
    }]);
    setPendingConnection(null);
  }, [nodes, handleEdgeBendChange]);

  const handleCharacterSaved = useCallback((updated: CharacterDto) => {
    const updatedChars = (tree?.characters ?? []).map(e =>
      e.character.id === updated.id ? updated : e.character
    );
    setTree(prev => {
      if (!prev) return prev;
      return { ...prev, characters: prev.characters.map(e => e.character.id === updated.id ? { ...e, character: updated } : e) };
    });
    setNodes(prev => prev.map(n =>
      n.id === updated.id ? { ...n, data: { ...n.data, character: updated } } : n
    ));
    setEdges(prev => {
      const nonParentEdges = prev.filter(e => !e.id.startsWith('parent-'));
      return [...nonParentEdges, ...buildParentEdges(updatedChars, handleParentBendsChange)];
    });
    setEditingCharacter(null);
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
          {tree && tree.characters.length === 0 && (
            <Box textAlign="center" mt={8}>
              <Typography color="text.secondary" gutterBottom>No characters in this tree yet.</Typography>
              <Button variant="outlined" onClick={() => setAddToTreeOpen(true)}>Add to Tree</Button>
            </Box>
          )}

          {tree && tree.characters.length > 0 && (
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
          existingCharacterIds={tree.characters.map(e => e.character.id)}
          onClose={() => setAddToTreeOpen(false)}
          onAdded={handleCharacterAddedToTree}
        />
      )}

      {editingCharacter && tree && (
        <EditCharacterDialog
          open
          character={editingCharacter}
          worldCharacters={worldCharacters}
          worldEntities={worldEntities}
          worldId={tree.worldId}
          onClose={() => setEditingCharacter(null)}
          onSaved={handleCharacterSaved}
          onDeleted={handleCharacterDeleted}
        />
      )}

      {pendingConnection && (
        <AddRelationshipDialog
          open
          character1={pendingConnection.character1}
          character2={pendingConnection.character2}
          onClose={() => setPendingConnection(null)}
          onCreated={handleRelationshipCreated}
        />
      )}
    </>
  );
}
