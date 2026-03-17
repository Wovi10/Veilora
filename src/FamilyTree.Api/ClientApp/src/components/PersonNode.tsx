import React, { useState } from 'react';
import { Handle, Position } from 'reactflow';
import type { NodeProps } from 'reactflow';
import Box from '@mui/material/Box';
import IconButton from '@mui/material/IconButton';
import Paper from '@mui/material/Paper';
import Typography from '@mui/material/Typography';
import EditIcon from '@mui/icons-material/Edit';
import MaleIcon from '@mui/icons-material/Male';
import FemaleIcon from '@mui/icons-material/Female';
import TransgenderIcon from '@mui/icons-material/Transgender';
import QuestionMarkIcon from '@mui/icons-material/QuestionMark';
import type { Gender, PersonDto } from '../types/person';

const genderIconMap: Record<Gender, React.ReactElement> = {
  Male: <MaleIcon sx={{ fontSize: 14, color: 'info.main' }} />,
  Female: <FemaleIcon sx={{ fontSize: 14, color: 'error.light' }} />,
  Other: <TransgenderIcon sx={{ fontSize: 14, color: 'secondary.main' }} />,
  Unknown: <QuestionMarkIcon sx={{ fontSize: 14, color: 'text.disabled' }} />,
};

function PersonNode({ data }: NodeProps<{ person: PersonDto; onEdit: (person: PersonDto) => void }>) {
  const { person, onEdit } = data;
  const [hovered, setHovered] = useState(false);

  const fullName = [person.firstName, person.middleName, person.lastName]
    .filter(Boolean)
    .join(' ');

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
        }}
      >
        <Box sx={{ position: 'absolute', top: 4, left: 6, lineHeight: 0 }}>
          {genderIconMap[person.gender]}
        </Box>
        <Typography variant="body2" fontWeight="bold" noWrap sx={{ maxWidth: '100%' }}>
          {fullName}
        </Typography>
        {(person.birthDate || person.deathDate) && (
          <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center', lineHeight: 1.3 }}>
            {person.birthDate && (
              <Typography variant="caption" color="text.secondary">
                ° {fmt(person.birthDate)}
              </Typography>
            )}
            {person.deathDate && (
              <Typography variant="caption" color="text.secondary">
                † {fmt(person.deathDate)}
              </Typography>
            )}
          </Box>
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
    </>
  );
}

export default React.memo(PersonNode);
