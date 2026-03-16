import 'reactflow/dist/style.css';
import { useEffect, useState, useCallback } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import ReactFlow, { Background, Controls, useNodesState } from 'reactflow';
import type { Node, Edge, NodeMouseHandler } from 'reactflow';
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
import AddPersonDialog from '../components/AddPersonDialog';
import EditPersonDialog from '../components/EditPersonDialog';

const COLS = 4;
const NODE_W = 180;
const NODE_H = 100;
const GAP = 60;

const nodeTypes = { person: PersonNode };

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

function buildEdges(relationships: RelationshipDto[]): Edge[] {
  return relationships.map((rel) => ({
    id: rel.id,
    source: rel.person1Id,
    target: rel.person2Id,
    type: 'smoothstep',
    label: rel.relationshipType,
  }));
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

  const handleEdit = useCallback((person: PersonDto) => {
    setEditingPerson(person);
  }, []);

  useEffect(() => {
    if (!id) return;
    Promise.all([getTreeDetails(id), getTreeRelationships(id)])
      .then(([treeData, rels]) => {
        setTree(treeData);
        setNodes(buildNodes(treeData.persons, handleEdit));
        setEdges(buildEdges(rels));
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
                onNodesChange={onNodesChange}
                onNodeDragStop={handleNodeDragStop}
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
          onClose={() => setEditingPerson(null)}
          onSaved={handlePersonSaved}
        />
      )}
    </>
  );
}
