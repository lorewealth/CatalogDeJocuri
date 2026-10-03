import {normalizeRatingSource, normalizeScore} from "@tools/ITADTools";

export interface ITADReview
{
	score: number;
	source: string;
	count: number;
	url: string;
}

export interface ITADGameData
{
	externalId: string;
	title: string;
	price: number;
	currency: string;
	stores: string[];
	scores: ITADReview[];
}

export async function searchITAD(name: string, country: string, env: Env): Promise<ITADGameData | null>
{
	//SEARCH GAME
	const searchURL =
		`https://api.isthereanydeal.com/games/search/v1` +
		`?key=${encodeURIComponent(env.ITAD_API_KEY)}` +
		`&title=${encodeURIComponent(name)}`;
	
	const searchResponse = await fetch(searchURL);
	
	if (!searchResponse.ok) throw new Error(`ITAD search failed: ${searchResponse.status}`);

	const searchJSON = await searchResponse.json() as Array<{id:string; title:string;}>;
	
	if (searchJSON.length === 0) return null;
	
	const externalId = searchJSON[0].id;
	const title = searchJSON[0].title;
	
	// INFO / REVIEWS
	const infoURL =
		`https://api.isthereanydeal.com/games/info/v2` +
		`?key=${encodeURIComponent(env.ITAD_API_KEY)}` +
		`&id=${encodeURIComponent(externalId)}`;
		
	const infoResponse = await fetch(infoURL);
	
	if (!infoResponse.ok) throw new Error(`ITAD info req failed: ${infoResponse.status}`);
	
	const infoJSON = await infoResponse.json() as {
		reviews?: Array<{
			score: number;
			source: string;
			count: number;
			url: string;
		}>;
	};
	
	const scores: ITADReview[] = [];
	
	for (const review of infoJSON.reviews ?? [])
	{
		const source = normalizeRatingSource(review.source);
		
		if (!source) continue;
		
		scores.push({
			score: normalizeScore(source, review.score),
			source,
			count: review.count,
			url: review.url
		});
	}
	
	//GET PRICES
	const priceResponse = await fetch
	(
		`https://api.isthereanydeal.com/games/prices/v3` +
		`?key=${encodeURIComponent(env.ITAD_API_KEY)}` +
		`&country=${encodeURIComponent(country)}`,
		{
			method: "POST",
			headers: { "Content-Type": "application/json" },
			body: JSON.stringify([externalId])
		}
	);
	
	if (!priceResponse.ok) throw new Error(`ITAD price request failed: ${priceResponse.status}`); 
	
	const priceJSON = await priceResponse.json() as Array<{
		id: string;
		deals: Array<{
			shop: {name:string;};
			price: {amount: number; currency: string;};
		}>;
	}>;
	
	const deals = priceJSON[0]?.deals ?? [];
	
	let price = 0;
	let currency = "";
	const stores: string[] = [];
	
	//Steam is preferred
	const selectedDeal = deals.find(deal => deal.shop.name.toLowerCase() === "steam") ?? deals[0];
	
	if (selectedDeal){
		price = selectedDeal.price.amount;
		currency = selectedDeal.price.currency;
	}
	
	for (const deal of deals)
	{
		if (!stores.includes(deal.shop.name)) stores.push(deal.shop.name);
	}
	
	return {
		externalId,
		title,
		price,
		currency,
		stores,
		scores
	};
}