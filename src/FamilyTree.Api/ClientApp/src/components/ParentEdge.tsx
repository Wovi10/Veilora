import type { EdgeProps } from 'reactflow';
import { useStore } from 'reactflow';

interface Data {
  siblingParentId?: string;
}

/**
 * Draws an H-bridge parent-child edge:
 *   - Each parent drops a vertical stub to a shared horizontal bar
 *   - A single vertical line drops from the bar's midpoint to the child
 *
 * When only one parent is known, falls back to a simple straight line.
 */
export default function ParentEdge({
  id,
  sourceX,
  sourceY,
  targetX,
  targetY,
  data,
}: EdgeProps<Data>) {
  const siblingNode = useStore(s =>
    data?.siblingParentId ? s.nodeInternals.get(data.siblingParentId) : undefined
  );

  let d: string;

  if (siblingNode) {
    const sibX = (siblingNode.positionAbsolute?.x ?? siblingNode.position.x)
      + (siblingNode.width ?? 180) / 2;
    const sibY = (siblingNode.positionAbsolute?.y ?? siblingNode.position.y)
      + (siblingNode.height ?? 80);

    const midX = (sourceX + sibX) / 2;
    const barY = Math.max(sourceY, sibY) + 20;

    // source → down to bar → horizontal to midX → down to child
    d = [
      `M ${sourceX} ${sourceY}`,
      `L ${sourceX} ${barY}`,
      `L ${midX} ${barY}`,
      `L ${midX} ${targetY}`,
      `L ${targetX} ${targetY}`,
    ].join(' ');
  } else {
    d = `M ${sourceX} ${sourceY} L ${targetX} ${targetY}`;
  }

  return (
    <path
      id={id}
      className="react-flow__edge-path"
      d={d}
      fill="none"
    />
  );
}
