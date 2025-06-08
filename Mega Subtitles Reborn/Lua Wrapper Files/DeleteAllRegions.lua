-- DeleteAllRegions.lua
local DeleteAllRegions = {}

function DeleteAllRegions.delete()
	local project_ID, _ = reaper.EnumProjects(-1, "") -- Get the current project ID
	local _, _, num_markers_regions = reaper.CountProjectMarkers(project_ID) -- Count the number of markers and regions in the project
	
    for i = num_markers_regions - 1, 0, -1 do -- Iterate through all markers and regions in reverse order
        local _, isrgn, _, _, _, region_idx = reaper.EnumProjectMarkers(i) -- Get the marker/region information
        
		if isrgn then -- If its region             
			reaper.DeleteProjectMarker(project_ID, region_idx, isrgn) -- Delete the specified project marker or region
        end
    end	
end 

return DeleteAllRegions