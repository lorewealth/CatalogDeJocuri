export function unixToDate(time: number): string
{
	return new Date(time * 1000).toISOString().slice(0, 10);
}
