import type { EdgeProps } from 'reactflow';
import { EdgeLabelRenderer, useStore } from 'reactflow';
import { useState } from 'react';
import Box from '@mui/material/Box';
import AutoAwesomeIcon from '@mui/icons-material/AutoAwesome';
import DiamondIcon from '@mui/icons-material/Diamond';
import FavoriteBorderIcon from '@mui/icons-material/FavoriteBorder';
import FavoriteIcon from '@mui/icons-material/Favorite';
import HandshakeIcon from '@mui/icons-material/Handshake';
import ShieldIcon from '@mui/icons-material/Shield';
import type { RelationshipType } from '../types/relationship';

interface Bend { x: number; y: number }

interface Data {
  relationshipType: RelationshipType;
  bends?: Bend[];
  onBendChange?: (edgeId: string, bends: Bend[]) => void;
}

function RelationshipIcon({ type }: { type: RelationshipType }) {
  switch (type) {
    case 'Spouse':
      return (
        <Box sx={{ display: 'flex', alignItems: 'center', gap: '1px' }}>
          <FavoriteIcon sx={{ fontSize: 13, color: '#e91e63' }} />
          <DiamondIcon sx={{ fontSize: 10, color: '#1976d2', mb: '1px' }} />
        </Box>
      );
    case 'Partner':
      return <FavoriteBorderIcon sx={{ fontSize: 14, color: '#e91e63' }} />;
    case 'Godparent':
      return <AutoAwesomeIcon sx={{ fontSize: 14, color: '#ff9800' }} />;
    case 'Guardian':
      return <ShieldIcon sx={{ fontSize: 14, color: '#607d8b' }} />;
    case 'CloseFriend':
      return <HandshakeIcon sx={{ fontSize: 14, color: '#4caf50' }} />;
  }
}

/** Closest distance from point (px,py) to segment (a→b). */
function segmentDist(px: number, py: number, a: Bend, b: Bend): number {
  const dx = b.x - a.x, dy = b.y - a.y;
  const len2 = dx * dx + dy * dy;
  if (len2 === 0) return Math.hypot(px - a.x, py - a.y);
  const t = Math.max(0, Math.min(1, ((px - a.x) * dx + (py - a.y) * dy) / len2));
  return Math.hypot(px - (a.x + t * dx), py - (a.y + t * dy));
}

/**
 * Straight relationship edge.
 * • Click anywhere on the line to add a waypoint at that spot.
 * • Drag a waypoint dot to move it.
 * • Hover a waypoint dot to reveal a × button that removes it.
 */
export default function RelationshipEdge({
  id,
  sourceX,
  sourceY,
  targetX,
  targetY,
  data,
}: EdgeProps<Data>) {
  const transform = useStore(s => s.transform);
  // { index, x, y } while a waypoint is being dragged; undefined otherwise
  const [dragging, setDragging] = useState<{ index: number; x: number; y: number } | undefined>();
  const [hoveredIndex, setHoveredIndex] = useState<number | null>(null);

  const storedBends: Bend[] = data?.bends ?? [];

  // Overlay the in-progress drag position on the stored bends
  const activeBends: Bend[] = storedBends.map((b, i) =>
    dragging?.index === i ? { x: dragging.x, y: dragging.y } : b
  );

  const d = [
    `M ${sourceX} ${sourceY}`,
    ...activeBends.map(b => `L ${b.x} ${b.y}`),
    `L ${targetX} ${targetY}`,
  ].join(' ');

  // All polyline vertices (for segment hit-testing)
  const allPoints: Bend[] = [{ x: sourceX, y: sourceY }, ...activeBends, { x: targetX, y: targetY }];

  /** Convert client coordinates to React Flow graph coordinates. */
  function toFlow(clientX: number, clientY: number): Bend {
    const [tx, ty, zoom] = transform;
    const rect = document.querySelector('.react-flow')?.getBoundingClientRect();
    return {
      x: rect ? (clientX - rect.left - tx) / zoom : clientX,
      y: rect ? (clientY - rect.top - ty) / zoom : clientY,
    };
  }

  /** Click on the edge → insert a new waypoint on the nearest segment. */
  function handlePathClick(e: React.MouseEvent<SVGPathElement>) {
    e.stopPropagation();
    const fp = toFlow(e.clientX, e.clientY);

    // Find nearest segment and insert before its end point
    let bestSeg = 0, minDist = Infinity;
    for (let i = 0; i < allPoints.length - 1; i++) {
      const dist = segmentDist(fp.x, fp.y, allPoints[i], allPoints[i + 1]);
      if (dist < minDist) { minDist = dist; bestSeg = i; }
    }

    const newBends = [...storedBends];
    newBends.splice(bestSeg, 0, fp);
    data?.onBendChange?.(id, newBends);
  }

  /** Mousedown on a waypoint dot → start dragging it. */
  function startDrag(index: number, e: React.MouseEvent) {
    e.stopPropagation();
    e.preventDefault();

    const startClientX = e.clientX;
    const startClientY = e.clientY;
    const origin = storedBends[index];
    const [, , zoom] = transform; // capture zoom at drag start

    function onMouseMove(ev: MouseEvent) {
      setDragging({
        index,
        x: origin.x + (ev.clientX - startClientX) / zoom,
        y: origin.y + (ev.clientY - startClientY) / zoom,
      });
    }

    function onMouseUp(ev: MouseEvent) {
      const final: Bend = {
        x: origin.x + (ev.clientX - startClientX) / zoom,
        y: origin.y + (ev.clientY - startClientY) / zoom,
      };
      setDragging(undefined);
      data?.onBendChange?.(id, storedBends.map((b, i) => i === index ? final : b));
      document.removeEventListener('mousemove', onMouseMove);
      document.removeEventListener('mouseup', onMouseUp);
    }

    document.addEventListener('mousemove', onMouseMove);
    document.addEventListener('mouseup', onMouseUp);
  }

  /** × button click → remove this waypoint. */
  function deleteBend(index: number, e: React.MouseEvent) {
    e.stopPropagation();
    data?.onBendChange?.(id, storedBends.filter((_, i) => i !== index));
  }

  const iconX = (sourceX + targetX) / 2;
  const iconY = (sourceY + targetY) / 2;

  return (
    <>
      {/* Wide transparent hit area so the edge is easy to click */}
      <path
        d={d}
        fill="none"
        stroke="transparent"
        strokeWidth={12}
        style={{ cursor: 'crosshair' }}
        onClick={handlePathClick}
      />

      {/* Visible edge line */}
      <path id={id} className="react-flow__edge-path" d={d} fill="none" />

      {/* Relationship type icon at the geometric midpoint */}
      {data?.relationshipType && (
        <EdgeLabelRenderer>
          <div
            style={{
              position: 'absolute',
              transform: `translate(-50%, -100%) translate(${iconX}px, ${iconY - 4}px)`,
              pointerEvents: 'none',
              display: 'flex',
              alignItems: 'center',
            }}
          >
            <RelationshipIcon type={data.relationshipType} />
          </div>
        </EdgeLabelRenderer>
      )}

      {/* One draggable handle per waypoint */}
      {activeBends.map((bend, i) => (
        <EdgeLabelRenderer key={i}>
          <div
            style={{
              position: 'absolute',
              transform: `translate(-50%, -50%) translate(${bend.x}px, ${bend.y}px)`,
              pointerEvents: 'all',
              display: 'flex',
              alignItems: 'center',
              gap: 3,
            }}
            onMouseEnter={() => setHoveredIndex(i)}
            onMouseLeave={() => setHoveredIndex(null)}
          >
            {/* Drag dot */}
            <div
              onMouseDown={e => startDrag(i, e)}
              style={{
                width: 10,
                height: 10,
                borderRadius: '50%',
                background: '#666',
                border: '1.5px solid #999',
                cursor: dragging?.index === i ? 'grabbing' : 'grab',
                flexShrink: 0,
              }}
            />

            {/* Delete button — only when hovered and not mid-drag */}
            {hoveredIndex === i && dragging?.index !== i && (
              <div
                onMouseDown={e => e.stopPropagation()}
                onClick={e => deleteBend(i, e)}
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
