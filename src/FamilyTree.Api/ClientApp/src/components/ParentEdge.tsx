import type { EdgeProps } from 'reactflow';
import { useStore } from 'reactflow';

interface Data {
  siblingParentId?: string;
  allChildIds?: string[];
  silent?: boolean;
}

/**
 * Draws a shared parent bar with all children hanging from it:
 *
 *   [Parent1]       [Parent2]
 *       |                 |
 *       └────────┬────────┘   ← shared bar
 *                |
 *       ┌────────┼────────┐   ← distribution line (multiple children)
 *       |        |        |
 *   [Child1]  [Child2]  [Child3]
 *
 * One primary edge (parent1 → firstChild) carries allChildIds and does all drawing.
 * All other parent→child edges are marked silent and render nothing.
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

  const childNodes = useStore(s => {
    if (data?.silent || !data?.allChildIds) return [];
    return data.allChildIds
      .map(cid => s.nodeInternals.get(cid))
      .filter((n): n is NonNullable<typeof n> => n != null);
  });

  if (data?.silent) {
    return <path id={id} className="react-flow__edge-path" d="M 0 0" fill="none" />;
  }

  let d: string;

  if (siblingNode) {
    const sibX = (siblingNode.positionAbsolute?.x ?? siblingNode.position.x)
      + (siblingNode.width ?? 180) / 2;
    const sibY = (siblingNode.positionAbsolute?.y ?? siblingNode.position.y)
      + (siblingNode.height ?? 80);

    const midX = (sourceX + sibX) / 2;
    const parentsBottomY = Math.max(sourceY, sibY);

    if (childNodes.length > 0) {
      const childPositions = childNodes.map(n => ({
        x: (n.positionAbsolute?.x ?? n.position.x) + (n.width ?? 180) / 2,
        y: n.positionAbsolute?.y ?? n.position.y,
      }));

      const minChildTopY = Math.min(...childPositions.map(c => c.y));
      const barY = parentsBottomY + (minChildTopY - parentsBottomY) * 0.3;

      const segments: string[] = [
        `M ${sourceX} ${sourceY} L ${sourceX} ${barY}`,
        `M ${sibX} ${sibY} L ${sibX} ${barY}`,
        `M ${sourceX} ${barY} L ${sibX} ${barY}`,
      ];

      if (childNodes.length === 1) {
        segments.push(`M ${midX} ${barY} L ${targetX} ${targetY}`);
      } else {
        const distributionY = barY + (minChildTopY - barY) * 0.6;
        const sortedX = [...childPositions].sort((a, b) => a.x - b.x);
        segments.push(
          `M ${midX} ${barY} L ${midX} ${distributionY}`,
          `M ${sortedX[0].x} ${distributionY} L ${sortedX[sortedX.length - 1].x} ${distributionY}`,
          ...childPositions.map(c => `M ${c.x} ${distributionY} L ${c.x} ${c.y}`),
        );
      }

      d = segments.join(' ');
    } else {
      // Fallback: sibling known but child list unavailable
      const barY = parentsBottomY + (targetY - parentsBottomY) / 2;
      d = [
        `M ${sourceX} ${sourceY}`,
        `L ${sourceX} ${barY}`,
        `L ${midX} ${barY}`,
        `L ${midX} ${targetY}`,
        `L ${targetX} ${targetY}`,
      ].join(' ');
    }
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
