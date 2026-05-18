import Drawer from '@mui/material/Drawer';
import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import { useIsMobile } from '../../hooks/useBreakpoint';

interface Props {
  open: boolean;
  onClose: () => void;
  title?: string;
  children: React.ReactNode;
}

export default function BottomSheet({ open, onClose, title, children }: Props) {
  const isMobile = useIsMobile();

  return (
    <Drawer
      anchor="bottom"
      open={open}
      onClose={onClose}
      disableRestoreFocus={isMobile}
      PaperProps={{
        sx: {
          borderRadius: '16px 16px 0 0',
          maxHeight: '90vh',
          overflowY: 'auto',
        },
      }}
    >
      <Box sx={{ width: 40, height: 4, borderRadius: 2, bgcolor: 'grey.400', my: 1, mx: 'auto' }} />
      {title && (
        <Typography variant="h6" sx={{ px: 2, pb: 1 }}>{title}</Typography>
      )}
      <Box sx={{ px: 2, pb: 3 }}>
        {children}
      </Box>
    </Drawer>
  );
}
