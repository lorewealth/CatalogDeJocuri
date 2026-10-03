export function normalizeRatingSource(source: string): string | null
{
	switch(source.toLowerCase())
	{
		case "steam": return "Steam";
		case "metascore": return "Metascore";
		case "metacritic user score": return "Metacritic_User_Score";
		case "opencritic": return "OpenCritic";
		
		default: return null;
	}
}

export function normalizeScore(source: string, score: number): number
{
	switch(source)
	{
		case "Steam": return score/20;
		case "Metacritic_User_Score": return score/10;
		case "Metascore":
		case "OpenCritic":
			return score;
			
		default: return score;
	}
}
