import os

for file in os.listdir("."):
  name, ext = os.path.splitext(file)
  if ext == ".opus":
    os.system("ffmpeg -i {0} -c:a libvorbis -y {1}".format(file, name + ".ogg"))