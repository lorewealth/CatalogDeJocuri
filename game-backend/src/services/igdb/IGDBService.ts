import { getIGDBToken } from "./IGDBAuthService";
import { unixToDate } from "@tools/DateTools";
import { normalizeAgeRating } from "@tools/IGDBTools";

export interface IGDBSearchResult
{
	id: number;
	name: string;
	first_release_date?: number;
}

export interface IGDBGameData
{
	externalId: number;
	title: string;
	releaseDate?: string;
	imgUrl?: string;
	
	genres: string[];
	developers: string[];
	publishers: string[];
	
	ageRatings: {
		source: string;
		value: string;
	}[];
}

interface IGDBRawGame
{
	id: number;
	name: string;
	first_release_date?: number;

	genres?: Array<{
		name: string;
	}>;

	involved_companies?: Array<{
		company: {
			name: string;
		};
		developer: boolean;
		publisher: boolean;
	}>;

	cover?: {
		url: string;
	};

	age_ratings?: Array<{
		organization: {
			name: string;
		};
		rating_category: {
			rating: string;
		};
	}>;
}

export async function getIGDBGame(
	id: number,
	env: Env
): Promise<IGDBGameData | null>
{
	const token = await getIGDBToken(env);
	
	const response = await fetch(
		"https://api.igdb.com/v4/games",
		{
			method: "POST",
			headers: {
				"Client-ID": env.IGDB_CLIENT_ID,
				"Authorization": `Bearer ${token}`,
				"Content-Type": "text/plain"
			},
			body: `
				where id = ${id};
				fields
					id,
					name,
					genres.name,
					involved_companies.company.name,
					involved_companies.developer,
					involved_companies.publisher,
					first_release_date,
					cover.url,
					age_ratings.organization.name,
					age_ratings.rating_category.rating;
				limit 1;
			`
		}
	);
	
	if (!response.ok)
		throw new Error(`IGDB failed: ${response.status}`);
	
	const games = await response.json() as IGDBRawGame[];
	
	if (games.length === 0)
		return null;
	
	const game = games[0];

	const ageRatings = game.age_ratings
		?.map(x =>
			normalizeAgeRating(
				x.organization.name,
				x.rating_category.rating
			)
		)
		.filter(x => x !== null)
		?? [];
	
	return {
		externalId: game.id,
		title: game.name,
		
		releaseDate: game.first_release_date
			? unixToDate(game.first_release_date)
			: undefined,
		
		imgUrl: game.cover?.url
			? `https:${game.cover.url}`
			: undefined,
		
		genres:
			game.genres?.map(x => x.name)
			?? [],
		
		developers:
			game.involved_companies
				?.filter(x => x.developer)
				.map(x => x.company.name)
			?? [],
		
		publishers:
			game.involved_companies
				?.filter(x => x.publisher)
				.map(x => x.company.name)
			?? [],
		
		ageRatings
	};
}

export async function searchIGDB(
	name: string,
	env: Env
): Promise<IGDBSearchResult[]>
{
	const token = await getIGDBToken(env);
	
	const safeName = name
		.replaceAll("\\", "\\\\")
		.replaceAll("\"", "\\\"");
	
	const response = await fetch(
		"https://api.igdb.com/v4/games",
		{
			method: "POST",
			headers: {
				"Client-ID": env.IGDB_CLIENT_ID,
				"Authorization": `Bearer ${token}`,
				"Content-Type": "text/plain"
			},
			body: `
				search "${safeName}";
				fields
					id,
					name,
					first_release_date;
				limit 10;
			`
		}
	);
	
	if (!response.ok)
		throw new Error(`IGDB failed: ${response.status}`);
	
	return await response.json() as IGDBSearchResult[];
}