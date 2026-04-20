export interface ReadingSessionDto {
  id: string;
  worldId: string;
  worldName: string;
  startedAt: string;
  endedAt?: string;
  noteCount: number;
}

export interface ReadingNoteDto {
  id: string;
  sessionId: string;
  text: string;
  tags: string[];
  createdAt: string;
}

export interface CreateReadingSessionDto {
  worldId: string;
}

export interface CreateReadingNoteDto {
  text: string;
}
