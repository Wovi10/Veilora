import { Card, CardActionArea, CardContent, Chip, Typography } from '@mui/material';
import type { CharacterDto } from '../types/character';

export default function CharacterCard({ character, onClick }: { character: CharacterDto; onClick: () => void }) {
  return (
    <Card sx={{ borderRadius: 2, height: '100%', transition: 'box-shadow 0.2s', '&:hover': { boxShadow: 3 } }}>
      <CardActionArea onClick={onClick} sx={{ height: '100%' }}>
        <CardContent sx={{ pb: '12px !important' }}>
          <Typography variant="subtitle1" fontWeight={600} noWrap>{character.name}</Typography>
          {character.species && (
            <Chip label={character.species} size="small" variant="outlined" sx={{ mt: 0.5, mr: 0.5 }} />
          )}
          {character.birthDate && (
            <Typography variant="caption" color="text.secondary" display="block" mt={0.5}>
              °&nbsp;{new Date(character.birthDate).toLocaleDateString('en-GB')}{character.birthDateSuffixAbbreviation && ` ${character.birthDateSuffixAbbreviation}`}
              {character.deathDate && ` — †\u00a0${new Date(character.deathDate).toLocaleDateString('en-GB')}${character.deathDateSuffixAbbreviation ? ` ${character.deathDateSuffixAbbreviation}` : ''}`}
            </Typography>
          )}
          {character.description && !character.birthDate && (
            <Typography
              variant="body2"
              color="text.secondary"
              sx={{ mt: 0.5, display: '-webkit-box', WebkitLineClamp: 2, WebkitBoxOrient: 'vertical', overflow: 'hidden' }}
            >
              {character.description}
            </Typography>
          )}
          <Typography variant="caption" color="text.secondary" display="block" mt={0.5}>
            Updated {new Date(character.updatedAt).toLocaleDateString('en-GB')}
          </Typography>
        </CardContent>
      </CardActionArea>
    </Card>
  );
}
