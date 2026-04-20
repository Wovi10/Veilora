export interface WorldSearchItem {
  id: string;
  name: string;
}

export interface WorldSearchResult {
  characters:  WorldSearchItem[];
  locations:   WorldSearchItem[];
  groups:      WorldSearchItem[];
  events:      WorldSearchItem[];
  concepts:    WorldSearchItem[];
  familyTrees: WorldSearchItem[];
}
