local GetUniqueActorsFromCache = {}

function contains(list, x)
	for _, v in ipairs(list) do
		if v == x then return true end
	end
	return false
end

function GetUniqueActorsFromCache.getActors(subtitleEntries)
	uniqueActorsList = {} -- Initialize an empty table to store unique actors
	for Actor, _ in pairs(subtitleEntries) do -- Iterate through each actor in the subtitle entries
		if not contains(uniqueActorsList, Actor) then -- Check if the actor is not already in the unique list
			table.insert(uniqueActorsList, Actor) -- Add the actor to the unique list
		end
	end		
	return uniqueActorsList
end

return GetUniqueActorsFromCache