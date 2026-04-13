import React, { useState } from 'react';
import { Handle, Position } from 'reactflow';
import type { NodeProps } from 'reactflow';
import Box from '@mui/material/Box';
import IconButton from '@mui/material/IconButton';
import Paper from '@mui/material/Paper';
import Typography from '@mui/material/Typography';
import { keyframes } from '@mui/system';
import EditIcon from '@mui/icons-material/Edit';
import MaleIcon from '@mui/icons-material/Male';
import FemaleIcon from '@mui/icons-material/Female';
import TransgenderIcon from '@mui/icons-material/Transgender';
import QuestionMarkIcon from '@mui/icons-material/QuestionMark';
import type { EntityDto, Gender } from '../types/entity';
import { useEditMode } from '../context/EditModeContext';

const glowPulse = keyframes`
  0%, 100% { box-shadow: 0 0 6px 2px rgba(99, 179, 237, 0.7); }
  50%       { box-shadow: 0 0 18px 6px rgba(99, 179, 237, 1); }
`;

const genderIconMap: Record<Gender, React.ReactElement> = {
  Male:    <MaleIcon sx={{ fontSize: 14, color: 'info.main' }} />,
  Female:  <FemaleIcon sx={{ fontSize: 14, color: 'error.light' }} />,
  Other:   <TransgenderIcon sx={{ fontSize: 14, color: 'secondary.main' }} />,
  Unknown: <QuestionMarkIcon sx={{ fontSize: 14, color: 'text.disabled' }} />,
};

function EntityNode({ data }: NodeProps<{ entity: EntityDto; onEdit: (entity: EntityDto) => void; isNew?: boolean }>) {
  const { entity, onEdit, isNew } = data;
  const [hovered, setHovered] = useState(false);
  const { isEditMode } = useEditMode();

  const displayName = (entity.firstName || entity.lastName)
    ? [entity.firstName, entity.lastName].filter(Boolean).join(' ')
    : entity.name;

  const fmt = (d: string) => new Date(d).toLocaleDateString('en-GB');

  return (
    <>
      <Handle type="source" position={Position.Top}    id="top"    />
      <Handle type="source" position={Position.Bottom} id="bottom" />
      <Handle type="source" position={Position.Left}   id="left"   />
      <Handle type="source" position={Position.Right}  id="right"  />
      <Paper
        variant="outlined"
        onMouseEnter={() => setHovered(true)}
        onMouseLeave={() => setHovered(false)}
        sx={{
          width: 180,
          height: 80,
          display: 'flex',
          flexDirection: 'column',
          justifyContent: 'center',
          alignItems: 'center',
          px: 1,
          textAlign: 'center',
          position: 'relative',
          ...(isNew && {
            borderColor: 'rgba(99, 179, 237, 0.9)',
            animation: `${glowPulse} 1.4s ease-in-out infinite`,
          }),
        }}
      >
        {entity.gender && (
          <Box sx={{ position: 'absolute', top: 4, left: 6, lineHeight: 0 }}>
            {genderIconMap[entity.gender]}
          </Box>
        )}
        <Typography variant="body2" fontWeight="bold" noWrap sx={{ maxWidth: '100%' }}>
          {displayName}
        </Typography>
        {(entity.birthDate || entity.deathDate) && (
          <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center', lineHeight: 1.3 }}>
            {entity.birthDate && (
              <Typography variant="caption" color="text.secondary">° {fmt(entity.birthDate)}</Typography>
            )}
            {entity.deathDate && (
              <Typography variant="caption" color="text.secondary">† {fmt(entity.deathDate)}</Typography>
            )}
          </Box>
        )}
        {hovered && isEditMode && (
          <IconButton
            size="small"
            onClick={e => { e.stopPropagation(); onEdit(entity); }}
            sx={{ position: 'absolute', top: 2, right: 2, p: '2px' }}
          >
            <EditIcon sx={{ fontSize: 14 }} />
          </IconButton>
        )}
      </Paper>
    </>
  );
}

export default React.memo(EntityNode);
