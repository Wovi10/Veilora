export type RelationshipType =
  | 'Spouse'
  | 'Partner'
  | 'Godparent'
  | 'Guardian'
  | 'CloseFriend';

export interface CreateRelationshipDto {
  person1Id: string;
  person2Id: string;
  relationshipType: RelationshipType;
  startDate?: string;
  endDate?: string;
  notes?: string;
}

export interface RelationshipDto {
  id: string;
  person1Id: string;
  person2Id: string;
  relationshipType: RelationshipType;
  startDate?: string;
  endDate?: string;
  notes?: string;
  createdAt: string;
  updatedAt: string;
}
