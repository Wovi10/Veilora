export type RelationshipType = 'Spouse' | 'Partner' | 'Godparent' | 'Guardian' | 'CloseFriend';

export interface RelationshipDto {
  id: string;
  character1Id: string;
  character2Id: string;
  relationshipType: RelationshipType;
  startDate?: string;
  endDate?: string;
  notes?: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateRelationshipDto {
  character1Id: string;
  character2Id: string;
  relationshipType: RelationshipType;
  startDate?: string;
  endDate?: string;
  notes?: string;
}
