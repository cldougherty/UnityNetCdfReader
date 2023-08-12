# Unity NetCDF/HDF Reader

This tool reads scientific data such as Carbon Emissions 
and Global Temperature from NetCDF/HDF5 files and generates a map as a texture from it

<img src="https://images.squarespace-cdn.com/content/v1/633e04028a555f5dcbfe3d29/bce57c65-a227-47db-96de-92ae7bd23d3f/netCdfUnity_Data.gif?format=750w" width="400">
<img src="https://images.squarespace-cdn.com/content/v1/633e04028a555f5dcbfe3d29/18ed3ea1-9a70-4d17-b4cc-ed51217c6f5c/netCdfUnity_Textures2.gif?format=750w" width="400">

Made for the IGDA Climate Change 2022 Game Jam

Unity version: 2021.3.8

Platform: Oculus Quest 2



Instructions: 
1.	Tools>Co2Tools
2.	Load dataset
3.	Select plot from list
4.	Choose colorramp texture
5.	Choose texture path to save (serializedObject gets saved here: Assets\Tools\HdfVisualizer)
6.	Add the serializedObject to the “DataSetViewer” object in the main scene


Tool notes:

Only reads files that were on the same drive as the unity installation

Right now every number in a dataset gets converted to a double. 

Built for datasets that are weekly, montly , etc. Datasets that use atrack/xtrack/FOV or Swath data are not supported,

Software such as Panoply use CF conventions to understand how to plot the data. For this project I only needed to read a few datasets(https://climate.nasa.gov/) so I check for certain Attributes manually.

Some datasets will need additional work to be read.

For the map Legend/Color ramp, Climatology already has a few standards using
Adobe Color Tables so I made a small plugin that converts it to a texture.


Built to read following datasets:

https://disc.gsfc.nasa.gov/datasets/GRACEDADM_CLSM025GL_7D_3.0/summary?keywords=GRACEDADM_CLSM025GL
https://podaac.jpl.nasa.gov/dataset/SEA_SURFACE_HEIGHT_ALT_GRIDS_L4_2SATS_5DAY_6THDEG_V_JPL2205
https://disc.gsfc.nasa.gov/datasets/SNDRAQIL3CMCCP_2/summary?keywords=SNDRAQIL3CMCCP_2
https://disc.gsfc.nasa.gov/datasets/OCO2_GEOS_L3CO2_MONTH_10r/summary?keywords=OCO2_GEOS_L3CO2_MONTH_10r

Images and more info here:
https://www.chrisdougherty.games/tools/netcdfhdf-reader-for-unity
