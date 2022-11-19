import os
import xml.etree.ElementTree as ET
#Place in media/packages. To work in separate mod folder, create vanilla/models folder and place vanilla .xml files there  
if __name__ == '__main__':
  # get current directory
  path = os.path.dirname(os.path.realpath(__file__))
  print("Current Directory", path)
 
  # prints parent directory
  parentDirectory = os.path.abspath(os.path.join(path, os.pardir))
  print(f"Parent directory(should point to media\packages): {parentDirectory}")

  #Declared here to avoid errors in debugmode
  targetColor = "0 0 0"
  targetLocation = "" #Will output directly to location of .py file
  targetFileName = "debug"  
  tc = [0, 0, 0]  

  debug = False  
  #Directory, filenames, and colors to replace of Greenbelt files specifically.
  copyfrom = "uniform_templates"
  copyFiles = ["soldier_a_ranger.xml", "soldier_a_vest_t3.xml", "soldier_a1_camo_vest.xml", "soldier_a1_vest.xml", "soldier_a1.xml", "soldier_a1eod.xml", "soldier_a2.xml", "soldier_a3.xml", "soldier_ag.xml", "soldier_am.xml", "soldier_ap.xml"]
  origColor = ["0.086581 0.163324 0.014294", "0.017658 0.272919 0.015227", "0.013989 0.349350 0.025738", "0.041959 0.332611 0.025259"]  
  
  #Gets input for outpot directory, model color, and file name
  if debug == False:
      targetLocation = input("Output directory (e.g my_mod/models):   ").lower().strip()
      targetColor = input("Target color (e.g 156 245 37): ").strip()
      targetFileName = input("Faction name to append to filename (e.g green):    ").strip()  
  else:
      print(os.listdir())  

  #Separates color values into array and converts to 0-1
  tc = targetColor.split(' ')
  tc =[float(s) for s in tc]
  newtc = []
  for s in tc:
      s /= 255
      newtc.append(s)
  tc = newtc
  tc =[str(s) for s in tc]  

  if targetLocation != "":
      targetLocation += "/" 
  parentDirectory = parentDirectory.replace('\\', "/")
  targetLocation = f"{parentDirectory}/{targetLocation}"

  for copyFile in copyFiles:
      #Reads, parses, and copies contents of file
      f = open(copyfrom + "/" + copyFile, 'r')
      content = ET.parse(f).getroot()
      content = ET.tostring(content, encoding='utf8', method='xml').decode("utf-8")
      print(f"Successfully read {copyFile}")
      f.close()  
      #Replaces original colors with new
      for oc in origColor:
          nc = oc.split(' ')
          content = content.replace(f'r="{nc[0]}"', f'r="{tc[0]}"')
          content = content.replace(f'g="{nc[1]}"', f'g="{tc[1]}"')
          content = content.replace(f'b="{nc[2]}"', f'b="{tc[2]}"')
      fileName = f"soldier_{targetFileName}{copyFile.split('a', 1)[1]}"  
      #Writes to/creates new file in target location
      if os.path.exists(targetLocation + fileName):
          a = open(targetLocation + fileName, 'w')
          a.truncate()
          a.write(str(content))
      else:
          print(f"File not found. Creating file....")
          with open(targetLocation + fileName, 'w') as nf:
              nf.truncate()
              nf.write(str(content))
      
      if os.path.exists(targetLocation + fileName):
          print(f"Successfully wrote {fileName}")
      else:
          print(f"Failed to write {fileName}")  
  print("All operations completed.")  
  input()
