param($installPath, $toolsPath, $package, $project)

function SetCopyToOutputDirectory($rootItem, [int]$value)
{
  $projectItems = $rootItem.ProjectItems

  foreach ($item in $projectItems)
  {
    if ($item.Kind -eq [EnvDTE.Constants]::vsProjectItemKindPhysicalFolder)
    {
      SetCopyToOutputDirectory $item $value
    }
    else
    {
      $item.Properties.Item("CopyToOutputDirectory").Value = $value
    }
  }
}

SetCopyToOutputDirectory $project.ProjectItems.Item("TRLIB") 2
