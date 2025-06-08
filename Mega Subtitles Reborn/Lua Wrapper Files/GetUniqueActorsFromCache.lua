local GetUniqueActorsFromCache = {}

function GetUniqueActorsFromCache.getActors(subtitleEntries)
	uniqueActorsList = {} -- Initialize an empty table to store unique actors
	for Actor, _ in pairs(subtitleEntries) do -- Iterate through each actor in the subtitle entries
		if not uniqueActorsList[Actor] then -- Check if the actor is not already in the unique list
			uniqueActorsList[Actor] = true -- Add the actor to the unique list
		end
	end
		
	return uniqueActorsList
end



return GetUniqueActorsFromCache