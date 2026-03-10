import 'reactflow/dist/style.css';
import { useEffect, useState, useCallback } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import ReactFlow, { Background, Controls } from 'reactflow';
import type { Node, Edge } from 'reactflow';
import {
  Alert,
  Box,
  Button,
  CircularProgress,
  IconButton,
  Typography,
} from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import { getTreeDetails, getTreeRelationships } from '../api/treeDetailApi';
import type { TreeWithPersonsDto } from '../types/tree';
import type { RelationshipDto } from '../types/relationship';
import type { PersonDto } from '../types/person';
import PersonNode from '../components/PersonNode';
import AddPersonDialog from '../components/AddPersonDialog';

const COLS = 4;
const NODE_W = 180;
const NODE_H = 100;
const GAP = 60;

const nodeTypes = { person: PersonNode };

function buildNodes(persons: PersonDto[]): Node[] {
  return persons.map((person, i) => ({
    id: person.id,
    type: 'person',
    position: {
      x: (i % COLS) * (NODE_W + GAP),
      y: Math.floor(i / COLS) * (NODE_H + GAP),
    },
    data: { person },
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
  const [nodes, setNodes] = useState<Node[]>([]);
  const [edges, setEdges] = useState<Edge[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [dialogOpen, setDialogOpen] = useState(false);

  useEffect(() => {
    if (!id) return;
    Promise.all([getTreeDetails(id), getTreeRelationships(id)])
      .then(([treeData, rels]) => {
        setTree(treeData);
        setNodes(buildNodes(treeData.persons));
        setEdges(buildEdges(rels));
      })
      .catch((err: unknown) =>
        setError(err instanceof Error ? err.message : 'Unexpected error')
      )
      .finally(() => setLoading(false));
  }, [id]);

  const handlePersonCreated = useCallback((person: PersonDto) => {
    setTree((prev) => {
      if (!prev) return prev;
      return { ...prev, persons: [...prev.persons, person] };
    });
    setNodes((prev) => {
      const next = [...prev, {
        id: person.id,
        type: 'person',
        position: {
          x: (prev.length % COLS) * (NODE_W + GAP),
          y: Math.floor(prev.length / COLS) * (NODE_H + GAP),
        },
        data: { person },
      }];
      return next;
    });
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
              <ReactFlow nodes={nodes} edges={edges} nodeTypes={nodeTypes} fitView>
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
    </>
  );
}
