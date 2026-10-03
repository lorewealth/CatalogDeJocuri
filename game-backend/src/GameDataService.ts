import { searchIGDB, getIGDBGame } from "@services/igdb/IGDBService";
import { searchITAD } from "@services/itad/ITADService";
import { unixToDate } from "@tools/DateTools";


export interface GameSearchResult
{
	igdbId: number;
	name: string;
	releaseDate?: string;
}

export interface GameData
{
	igdbId: number;
	itadId?: string;
	
	name: string;
	
	genres: string[];
	developers: string[];
	publishers: string[];
	
	ageRatings:{
		source: string;
		value: string;
	}[];
	scores:{
		source: string;
		score: number;
		count: number;
		url: string;
	}[];
	
	price: number;
	currency: string;
	stores: string[];
	
	releaseDate?: string;
	imgUrl?: string;
}

export async function searchGames(name: string, env: Env): Promise<GameSearchResult[]>
{
	const igdbData = await searchIGDB(name, env);
	
	return igdbData.map(game => ({
		igdbId: game.id,
		name: game.name,
		releaseDate: game.first_release_date
					? unixToDate(game.first_release_date):
					undefined,
	}));
}

export async function getGameData(igdbId: number, country: string, env: Env): Promise<GameData | null>
{
	const igdbData = await getIGDBGame(igdbId, env);
	
	if (!igdbData) return null;
	
	const itadData = await searchITAD(igdbData.title, country, env);
	
	return{
		igdbId: igdbData.externalId,
		itadId: itadData?.externalId,
		
		name: igdbData.title,
		
		genres: igdbData.genres,
		developers: igdbData.developers,
		publishers: igdbData.publishers,
		ageRatings: igdbData.ageRatings,
		
		scores: itadData?.scores ?? [],
		
		price: itadData?.price ?? 0,
		currency: itadData?.currency ?? "",
		stores: itadData?.stores ?? [],
		
		releaseDate: igdbData.releaseDate,
		imgUrl: igdbData.imgUrl
	};
}