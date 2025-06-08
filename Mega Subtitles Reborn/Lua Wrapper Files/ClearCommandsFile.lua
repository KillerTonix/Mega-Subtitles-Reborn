local ClearCommandsFile = {}

function ClearCommandsFile.Clear()
	local CommandsFile = io.open(ExecutableCommandsFilePath, "w") -- Open the commands file in write mode to clear its contents
    if CommandsFile then -- Check if the file was opened successfully
        CommandsFile:write("") -- Clear file contents
        CommandsFile:close() -- Close the file after clearing
    end
end

return ClearCommandsFile
