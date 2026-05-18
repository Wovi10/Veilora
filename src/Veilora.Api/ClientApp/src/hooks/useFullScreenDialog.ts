import { useIsMobile } from './useBreakpoint';

export default function useFullScreenDialog(): { fullScreen: boolean } {
  const fullScreen = useIsMobile();
  return { fullScreen };
}
