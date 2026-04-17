export interface NoteDto {
  id: string;
  content: string;
  worldId?: string;
  entityId?: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateNoteDto {
  content: string;
  worldId?: string;
  entityId?: string;
}
