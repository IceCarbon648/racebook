export const queryKeys = {
  me: ["me"] as const,
  mods: ["mods"] as const,
  modDetails: (modId: string) => ["modDetails", modId] as const,
  myMods: ["myMods"] as const,
  favourites: ["favourites"] as const,
  categories: ["categories"] as const,
};
