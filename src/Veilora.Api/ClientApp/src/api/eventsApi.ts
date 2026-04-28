import { apiFetch } from './apiFetch';
import type { EventDto, CreateEventDto, UpdateEventDto } from '../types/event';
import type { PagedResult } from '../types/paged';

export async function getEventsByWorld(worldId: string): Promise<EventDto[]> {
  const res = await apiFetch(`/api/events/world/${worldId}`);
  return res.json();
}

export async function getEventsByWorldPaged(worldId: string, page: number, pageSize: number, name?: string): Promise<PagedResult<EventDto>> {
  const url = `/api/events/world/${worldId}?page=${page}&pageSize=${pageSize}${name ? `&name=${encodeURIComponent(name)}` : ''}`;
  const res = await apiFetch(url);
  return res.json();
}

export async function getEvent(id: string): Promise<EventDto> {
  const res = await apiFetch(`/api/events/${id}`);
  return res.json();
}

export async function createEvent(dto: CreateEventDto): Promise<EventDto> {
  const res = await apiFetch('/api/events', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  });
  return res.json();
}

export async function updateEvent(id: string, dto: UpdateEventDto): Promise<EventDto> {
  const res = await apiFetch(`/api/events/${id}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(dto),
  });
  return res.json();
}

export async function deleteEvent(id: string): Promise<void> {
  await apiFetch(`/api/events/${id}`, { method: 'DELETE' });
}
