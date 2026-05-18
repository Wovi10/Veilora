import { useMediaQuery } from '@mui/material';
import { useTheme } from '@mui/material/styles';

export function useIsMobile(): boolean {
  const theme = useTheme();
  return useMediaQuery(theme.breakpoints.down('sm'));
}

export function useIsTablet(): boolean {
  const theme = useTheme();
  return useMediaQuery(theme.breakpoints.between('sm', 'md'));
}
