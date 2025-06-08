-- GetCurrentProjectName.lua
local GetCurrentProjectName = {}

function GetCurrentProjectName.getName()
	local projectPath = reaper.GetProjectPath() -- Get the current project path
	local project_ID, _ = reaper.EnumProjects(-1, "") -- Get the current project ID
	local projectName = reaper.GetProjectName(project_ID, "") -- Get project name with extension
	
	if #projectPath > 0 and projectName then
		local function getLastFoldersAndProject(path, projName) -- Function to get the last two folders and project name
			path = path:gsub("[/\\]Media$", "") -- Remove 'Media' from the end of the path
			
			local parts = {} -- Table to hold the parts of the path
			for part in path:gmatch("[^/\\]+") do -- Split the path into parts
				parts[#parts + 1] = part -- Add each part to the table
			end
			
			if #parts >= 2 then -- If there are at least two parts in the path
				return parts[#parts-1] .. "_" .. parts[#parts] .. "_" .. projName -- Return the last two folders and project name
			elseif #parts == 1 then -- If there is only one part in the path
				return parts[1] .. "_" .. projName -- Return the single folder and project name
			end
			return projName	-- If there are no folders, return just the project name
		end
		
		local formattedPath = getLastFoldersAndProject(projectPath, projectName) -- Format the path to include the last two folders and project name
		if formattedPath then -- Check if the formatted path is not nil
			local file = io.open(CurrentProjectNameFilePath, "w") -- Open the file for writing
			if file then -- If the file is opened successfully
				newProjectName = formattedPath:gsub(".rpp", "") -- Remove the .rpp extension
				file:write(newProjectName) -- Write the new project name to the file
				file:close() -- Close the file
				return newProjectName -- Return the new project name
			end
		end
	end
	
	-- Error handling
	reaper.ShowMessageBox("You have not saved the project.\nSave the project in Reaper and then run the script.", "Running Error", 0) -- Show an error message if the project is not saved
	os.execute("TASKKILL /IM 'Mega Subtitles Reborn.exe'") -- Kill the Mega Subtitles Reborn process
	os.execute("TASKKILL /IM 'Mega Subtitles Reborn.exe'") -- Kill the Mega Subtitles Reborn process
	os.execute("TASKKILL /IM 'Mega Subtitles Reborn.exe'") -- Kill the Mega Subtitles Reborn process

	gfx.quit() -- Close the gfx window

	return nil -- Return nil if the project name could not be retrieved
end

return GetCurrentProjectName
