export type RelationshipType = 'Spouse' | 'Partner' | 'Godparent' | 'Guardian' | 'CloseFriend';

export interface RelationshipDto {
  id: string;
  entity1Id: string;
  entity2Id: string;
  relationshipType: RelationshipType;
  startDate?: string;
  endDate?: string;
  notes?: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateRelationshipDto {
  entity1Id: string;
  entity2Id: string;
  relationshipType: RelationshipType;
  startDate?: string;
  endDate?: string;
  notes?: string;
}
