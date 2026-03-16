import React, { useState } from 'react';
import { Handle, Position } from 'reactflow';
import type { NodeProps } from 'reactflow';
import IconButton from '@mui/material/IconButton';
import Paper from '@mui/material/Paper';
import Typography from '@mui/material/Typography';
import EditIcon from '@mui/icons-material/Edit';
import type { PersonDto } from '../types/person';

function PersonNode({ data }: NodeProps<{ person: PersonDto; onEdit: (person: PersonDto) => void }>) {
  const { person, onEdit } = data;
  const [hovered, setHovered] = useState(false);

  const fullName = [person.firstName, person.middleName, person.lastName]
    .filter(Boolean)
    .join(' ');

  const birthYear = person.birthDate ? new Date(person.birthDate).getFullYear() : null;
  const deathYear = person.deathDate ? new Date(person.deathDate).getFullYear() : null;
  const years = birthYear
    ? deathYear
      ? `${birthYear} – ${deathYear}`
      : `b. ${birthYear}`
    : null;

  return (
    <>
      <Handle type="target" position={Position.Top} />
      <Paper
        variant="outlined"
        onMouseEnter={() => setHovered(true)}
        onMouseLeave={() => setHovered(false)}
        sx={{
          width: 160,
          height: 80,
          display: 'flex',
          flexDirection: 'column',
          justifyContent: 'center',
          alignItems: 'center',
          px: 1,
          textAlign: 'center',
          position: 'relative',
        }}
      >
        <Typography variant="body2" fontWeight="bold" noWrap sx={{ maxWidth: '100%' }}>
          {fullName}
        </Typography>
        {years && (
          <Typography variant="caption" color="text.secondary">
            {years}
          </Typography>
        )}
        {hovered && (
          <IconButton
            size="small"
            onClick={(e) => { e.stopPropagation(); onEdit(person); }}
            sx={{ position: 'absolute', top: 2, right: 2, p: '2px' }}
          >
            <EditIcon sx={{ fontSize: 14 }} />
          </IconButton>
        )}
      </Paper>
      <Handle type="source" position={Position.Bottom} />
    </>
  );
}

export default React.memo(PersonNode);
