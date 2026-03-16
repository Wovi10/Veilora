import type { EdgeProps } from 'reactflow';
import { EdgeLabelRenderer, getSmoothStepPath } from 'reactflow';
import Box from '@mui/material/Box';
import AutoAwesomeIcon from '@mui/icons-material/AutoAwesome';
import DiamondIcon from '@mui/icons-material/Diamond';
import FavoriteBorderIcon from '@mui/icons-material/FavoriteBorder';
import FavoriteIcon from '@mui/icons-material/Favorite';
import HandshakeIcon from '@mui/icons-material/Handshake';
import ShieldIcon from '@mui/icons-material/Shield';
import type { RelationshipType } from '../types/relationship';

interface Data {
  relationshipType: RelationshipType;
}

function RelationshipIcon({ type }: { type: RelationshipType }) {
  switch (type) {
    case 'Spouse':
      return (
        <Box sx={{ display: 'flex', alignItems: 'center', gap: '1px' }}>
          <FavoriteIcon sx={{ fontSize: 13, color: '#e91e63' }} />
          <DiamondIcon sx={{ fontSize: 10, color: '#1976d2', mb: '1px' }} />
        </Box>
      );
    case 'Partner':
      return <FavoriteBorderIcon sx={{ fontSize: 14, color: '#e91e63' }} />;
    case 'Godparent':
      return <AutoAwesomeIcon sx={{ fontSize: 14, color: '#ff9800' }} />;
    case 'Guardian':
      return <ShieldIcon sx={{ fontSize: 14, color: '#607d8b' }} />;
    case 'CloseFriend':
      return <HandshakeIcon sx={{ fontSize: 14, color: '#4caf50' }} />;
  }
}

export default function RelationshipEdge({
  id,
  sourceX,
  sourceY,
  targetX,
  targetY,
  sourcePosition,
  targetPosition,
  data,
}: EdgeProps<Data>) {
  const [edgePath, labelX, labelY] = getSmoothStepPath({
    sourceX,
    sourceY,
    sourcePosition,
    targetPosition,
    targetX,
    targetY,
  });

  return (
    <>
      <path id={id} className="react-flow__edge-path" d={edgePath} fill="none" />
      {data?.relationshipType && (
        <EdgeLabelRenderer>
          <div
            style={{
              position: 'absolute',
              transform: `translate(-50%, -100%) translate(${labelX}px, ${labelY - 4}px)`,
              pointerEvents: 'none',
              display: 'flex',
              alignItems: 'center',
            }}
          >
            <RelationshipIcon type={data.relationshipType} />
          </div>
        </EdgeLabelRenderer>
      )}
    </>
  );
}
