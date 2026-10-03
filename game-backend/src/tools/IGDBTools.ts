export function normalizeAgeRating(source: string, value: string): 
								  {source: string, value: string} | null
{
	switch(source)
	{
        case "ESRB":
            return {
                source: "ESRB",
                value: `ESRB_${value}`
            };

        case "PEGI":
            return {
                source: "PEGI",
                value: `PEGI${value}`
            };

        case "USK":
            return {
                source: "USK",
                value: `USK${value}`
            };

        case "CERO":
            return {
                source: "CERO",
                value: `CERO_${value}`
            };

        case "GRAC":
            return {
                source: "GRAC",
                value: `GRAC_${value.replace("+", "")}`
            };

        case "CLASS_IND":
            return {
                source: "CLASS_IND",
                value: `CLASS_IND_${value}`
            };

        case "ACB":
            return {
                source: "ACB",
                value: `ACB_${value.replace("+", "")}`
            };

        default:
            return null;
	}
}