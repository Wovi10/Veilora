import type { EdgeProps } from 'reactflow';
import { useStore, EdgeLabelRenderer } from 'reactflow';
import { useState } from 'react';

interface Bend { x: number; y: number }

interface Data {
  siblingParentId?: string;
  allChildIds?: string[];
  silent?: boolean;
  /** Waypoints per child drop, keyed by childId (or 'direct' for single-parent). */
  bends?: Record<string, Bend[]>;
  onBendsChange?: (edgeId: string, bends: Record<string, Bend[]>) => void;
}

/** Closest distance from point (px,py) to segment a→b. */
function segDist(px: number, py: number, a: Bend, b: Bend): number {
  const dx = b.x - a.x, dy = b.y - a.y;
  const len2 = dx * dx + dy * dy;
  if (len2 === 0) return Math.hypot(px - a.x, py - a.y);
  const t = Math.max(0, Math.min(1, ((px - a.x) * dx + (py - a.y) * dy) / len2));
  return Math.hypot(px - (a.x + t * dx), py - (a.y + t * dy));
}

function makePath(pts: Bend[]): string {
  return pts.map((p, i) => `${i === 0 ? 'M' : 'L'} ${p.x} ${p.y}`).join(' ');
}

/**
 * Draws the H-bridge parent–child layout with optional per-drop waypoints.
 *
 * One primary edge (parent1 → firstChild) renders everything.
 * All other parent→child edges are `silent` and render nothing.
 * Clicking any drop line inserts a waypoint at that position.
 */
export default function ParentEdge({
  id,
  sourceX,
  sourceY,
  targetX,
  targetY,
  data,
}: EdgeProps<Data>) {
  // --- all hooks first (React rules) ---
  const siblingNode = useStore(s =>
    data?.siblingParentId ? s.nodeInternals.get(data.siblingParentId) : undefined
  );
  const childNodes = useStore(s => {
    if (data?.silent || !data?.allChildIds) return [];
    return data.allChildIds
      .map(cid => s.nodeInternals.get(cid))
      .filter((n): n is NonNullable<typeof n> => n != null);
  });
  const transform = useStore(s => s.transform);

  const [dragging, setDragging] = useState<
    { key: string; index: number; x: number; y: number } | undefined
  >();
  const [hovered, setHovered] = useState<{ key: string; index: number } | null>(null);

  // --- silent guard (after all hooks) ---
  if (data?.silent) {
    return <path id={id} d="M 0 0" fill="none" />;
  }

  const stored: Record<string, Bend[]> = data?.bends ?? {};

  // Active bends for a key (overlays in-progress drag)
  function activeBends(key: string): Bend[] {
    const s = stored[key] ?? [];
    if (dragging?.key === key) {
      return s.map((b, i) => i === dragging.index ? { x: dragging.x, y: dragging.y } : b);
    }
    return s;
  }

  function toFlow(clientX: number, clientY: number): Bend {
    const [tx, ty, zoom] = transform;
    const rect = document.querySelector('.react-flow')?.getBoundingClientRect();
    return {
      x: rect ? (clientX - rect.left - tx) / zoom : clientX,
      y: rect ? (clientY - rect.top - ty) / zoom : clientY,
    };
  }

  function insertBend(key: string, pts: Bend[], e: React.MouseEvent<SVGPathElement>) {
    e.stopPropagation();
    const fp = toFlow(e.clientX, e.clientY);

    let bestSeg = 0, minDist = Infinity;
    for (let i = 0; i < pts.length - 1; i++) {
      const d = segDist(fp.x, fp.y, pts[i], pts[i + 1]);
      if (d < minDist) { minDist = d; bestSeg = i; }
    }

    const cur = stored[key] ?? [];
    const next = [...cur];
    next.splice(bestSeg, 0, fp);
    data?.onBendsChange?.(id, { ...stored, [key]: next });
  }

  function startDrag(key: string, index: number, e: React.MouseEvent) {
    e.stopPropagation();
    e.preventDefault();
    const startCX = e.clientX, startCY = e.clientY;
    const origin = (stored[key] ?? [])[index];
    const [,, zoom] = transform;

    function onMove(ev: MouseEvent) {
      setDragging({ key, index, x: origin.x + (ev.clientX - startCX) / zoom, y: origin.y + (ev.clientY - startCY) / zoom });
    }
    function onUp(ev: MouseEvent) {
      const final = { x: origin.x + (ev.clientX - startCX) / zoom, y: origin.y + (ev.clientY - startCY) / zoom };
      setDragging(undefined);
      data?.onBendsChange?.(id, { ...stored, [key]: (stored[key] ?? []).map((b, i) => i === index ? final : b) });
      document.removeEventListener('mousemove', onMove);
      document.removeEventListener('mouseup', onUp);
    }
    document.addEventListener('mousemove', onMove);
    document.addEventListener('mouseup', onUp);
  }

  function removeBend(key: string, index: number, e: React.MouseEvent) {
    e.stopPropagation();
    data?.onBendsChange?.(id, { ...stored, [key]: (stored[key] ?? []).filter((_, i) => i !== index) });
  }

  /** Renders the clickable hit path + visible path + waypoint handles for one drop segment. */
  function renderDrop(key: string, start: Bend, end: Bend) {
    const bends = activeBends(key);
    const pts = [start, ...bends, end];
    const d = makePath(pts);
    return (
      <>
        {/* Wide transparent hit area */}
        <path
          d={d}
          fill="none"
          stroke="transparent"
          strokeWidth={12}
          style={{ cursor: 'crosshair' }}
          onClick={e => insertBend(key, pts, e)}
        />
        {/* Visible drop line */}
        <path d={d} className="react-flow__edge-path" fill="none" />
        {/* Waypoint handles */}
        {bends.map((bend, i) => (
          <EdgeLabelRenderer key={`${key}-${i}`}>
            <div
              style={{
                position: 'absolute',
                transform: `translate(-50%, -50%) translate(${bend.x}px, ${bend.y}px)`,
                pointerEvents: 'all',
                display: 'flex',
                alignItems: 'center',
                gap: 3,
              }}
              onMouseEnter={() => setHovered({ key, index: i })}
              onMouseLeave={() => setHovered(null)}
            >
              <div
                onMouseDown={e => startDrag(key, i, e)}
                style={{
                  width: 10,
                  height: 10,
                  borderRadius: '50%',
                  background: '#666',
                  border: '1.5px solid #999',
                  cursor: dragging?.key === key && dragging?.index === i ? 'grabbing' : 'grab',
                  flexShrink: 0,
                }}
              />
              {hovered?.key === key && hovered?.index === i && dragging?.key !== key && (
                <div
                  onMouseDown={e => e.stopPropagation()}
                  onClick={e => removeBend(key, i, e)}
                  style={{
                    width: 14,
                    height: 14,
                    borderRadius: '50%',
                    background: '#e57373',
                    color: 'white',
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                    fontSize: 11,
                    lineHeight: 1,
                    cursor: 'pointer',
                    userSelect: 'none',
                    flexShrink: 0,
                  }}
                >
                  ×
                </div>
              )}
            </div>
          </EdgeLabelRenderer>
        ))}
      </>
    );
  }

  // ── Single parent: straight line with optional waypoints ──────────────────
  if (!siblingNode) {
    const bends = activeBends('direct');
    const pts = [{ x: sourceX, y: sourceY }, ...bends, { x: targetX, y: targetY }];
    const d = makePath(pts);
    return (
      <>
        {/* id path required by React Flow */}
        <path id={id} d={d} className="react-flow__edge-path" fill="none" style={{ pointerEvents: 'none' }} />
        {renderDrop('direct', { x: sourceX, y: sourceY }, { x: targetX, y: targetY })}
      </>
    );
  }

  // ── H-bridge: two parents ─────────────────────────────────────────────────
  const sibX = (siblingNode.positionAbsolute?.x ?? siblingNode.position.x) + (siblingNode.width ?? 180) / 2;
  const sibY = (siblingNode.positionAbsolute?.y ?? siblingNode.position.y) + (siblingNode.height ?? 80);
  const midX = (sourceX + sibX) / 2;
  const parentsBottomY = Math.max(sourceY, sibY);

  if (childNodes.length === 0) {
    // Fallback: sibling known but child list unavailable
    const barY = parentsBottomY + (targetY - parentsBottomY) / 2;
    const structD = [
      `M ${sourceX} ${sourceY} L ${sourceX} ${barY}`,
      `M ${sibX} ${sibY} L ${sibX} ${barY}`,
      `M ${sourceX} ${barY} L ${sibX} ${barY}`,
    ].join(' ');
    return (
      <>
        <path id={id} className="react-flow__edge-path" d={structD} fill="none" />
        {renderDrop('fallback', { x: midX, y: barY }, { x: targetX, y: targetY })}
      </>
    );
  }

  const childPositions = childNodes.map(n => ({
    id: n.id,
    x: (n.positionAbsolute?.x ?? n.position.x) + (n.width ?? 180) / 2,
    y: n.positionAbsolute?.y ?? n.position.y,
  }));
  const minChildTopY = Math.min(...childPositions.map(c => c.y));
  const barY = parentsBottomY + (minChildTopY - parentsBottomY) * 0.3;

  if (childNodes.length === 1) {
    // Single child: bar + one drop
    const structD = [
      `M ${sourceX} ${sourceY} L ${sourceX} ${barY}`,
      `M ${sibX} ${sibY} L ${sibX} ${barY}`,
      `M ${sourceX} ${barY} L ${sibX} ${barY}`,
    ].join(' ');
    const child = childPositions[0];
    return (
      <>
        <path id={id} className="react-flow__edge-path" d={structD} fill="none" />
        {renderDrop(child.id, { x: midX, y: barY }, { x: child.x, y: child.y })}
      </>
    );
  }

  // Multiple children: bar + distribution line + per-child drops
  const distributionY = barY + (minChildTopY - barY) * 0.6;
  const sortedX = [...childPositions].sort((a, b) => a.x - b.x);
  const structD = [
    `M ${sourceX} ${sourceY} L ${sourceX} ${barY}`,
    `M ${sibX} ${sibY} L ${sibX} ${barY}`,
    `M ${sourceX} ${barY} L ${sibX} ${barY}`,
    `M ${midX} ${barY} L ${midX} ${distributionY}`,
    `M ${sortedX[0].x} ${distributionY} L ${sortedX[sortedX.length - 1].x} ${distributionY}`,
  ].join(' ');

  return (
    <>
      <path id={id} className="react-flow__edge-path" d={structD} fill="none" />
      {childPositions.map(child =>
        renderDrop(child.id, { x: child.x, y: distributionY }, { x: child.x, y: child.y })
      )}
    </>
  );
}
