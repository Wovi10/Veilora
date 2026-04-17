export interface WorldPermissionDto {
  id: string;
  worldId: string;
  userId: string;
  username: string | null;
  canEdit: boolean;
}

export interface UpsertWorldPermissionDto {
  canEdit: boolean;
}
