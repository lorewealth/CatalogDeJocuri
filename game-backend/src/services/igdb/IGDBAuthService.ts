export async function getIGDBToken(env: Env): Promise<string>{
	const response = await fetch(
	`https://id.twitch.tv/oauth2/token?` +
	`client_id=${encodeURIComponent(env.IGDB_CLIENT_ID)}` +
	`&client_secret=${encodeURIComponent(env.IGDB_CLIENT_SECRET)}` +
	`&grant_type=client_credentials`,
	{ method: "POST" }
	);
	
	if (!response.ok) throw new Error(`Twitch auth failed: ${response.status}`);
	
	const data = await response.json() as { access_token: string; };
	
	return data.access_token;
}