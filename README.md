# WADExplorer
DDI Games .WAD archive files explorer, it can open .WAD files for Monster Trux: Extreme, Rig Racer 2, Offroad Extreme!, Ninja Breadman, etc.                        
Choose between two formats: old and new, because DDI has changed the .WAD files format between the developed games.
# Model Importing                 
Use WaveFront .OBJ format to import your custom formats,
## Materials         
Between materials make your material's name like this:                            
**Mat_materialID** -> **Mat_0**     
From a range of 0 to the ID of your material. (in a chronological order actually)
For example you want to define material 72:                     
**Mat_71**
### Textures
Using any text editor open the **.mtl** file of your .obj file,                
That's where the materials are stored into.                     

Now if you see full paths to the texture like this:                                        
**C:/My Folder/My Texture.png**                                        
You should change it to:                              
**Data/My Folder/My Texture.png** *(Include "Data" as root)*                                              
Otherwise the game won't **load** it because it wasn't found in the *.WAD* file.
## Vertices
*Using Blender 2.76...*                                                        

Once you're done with your model, Select all faces and go to:               
**Mesh > Edges >> Edge Split**, 
to make the edges splitted. (that's how RW DFF vertices work)

## Export Settings
- (YES) Include Edges                                   
- (YES) Triangulate Faces (only if you didn't triangulate in your model)                                 
- (NO) Write Normals (that might make your .obj weight a bit more in some KBs)                           
- (YES) Keep Vertex Order                                     

*...(The rest you don't do anything)* 
 
# License
*MIT License*                            

**Copyright (c) 2023 BuilderDemo7**                                                           

Permission is hereby granted, free of charge, to any person obtaining a copy                     
of this software and associated documentation files (the "Software"), to deal                         
in the Software without restriction, including without limitation the rights                        
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell                             
copies of the Software, and to permit persons to whom the Software is                                 
furnished to do so, subject to the following conditions:                                      

The above copyright notice and this permission notice shall be included in all                      
copies or substantial portions of the Software.                                       

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR                                   
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,                              
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE                                  
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER                               
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,                          
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE                                 
SOFTWARE.                                      
