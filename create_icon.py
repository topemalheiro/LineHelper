from PIL import Image, ImageDraw
import os

# Create a 256x256 icon with a marker design
size = 256
img = Image.new('RGBA', (size, size), (0, 0, 0, 0))
draw = ImageDraw.Draw(img)

# Background circle
center = size // 2
radius = 120
draw.ellipse([center-radius, center-radius, center+radius, center+radius],
             fill='#2196F3')

# Waveform (simplified as line segments)
wave_color = 'white'
wave_width = 8
points = [
    (center-70, center),
    (center-50, center-30),
    (center-30, center+20),
    (center-10, center-20),
    (center+10, center+30),
    (center+30, center-25),
    (center+50, center+15),
    (center+70, center)
]
draw.line(points, fill=wave_color, width=wave_width, joint='curve')

# Left marker (IN)
marker_color = '#FF9800'
marker_width = 6
left_x = center - 50
draw.line([(left_x, center-50), (left_x, center+50)], fill=marker_color, width=marker_width)
draw.ellipse([left_x-8, center-50-8, left_x+8, center-50+8], fill=marker_color)

# Right marker (OUT)
right_x = center + 50
draw.line([(right_x, center-50), (right_x, center+50)], fill=marker_color, width=marker_width)
draw.ellipse([right_x-8, center-50-8, right_x+8, center-50+8], fill=marker_color)

# Save as ICO (multiple sizes)
output_dir = r'C:\Users\topem\Desktop\Mochi Project\LineHelper\LineHelper\Resources'
os.makedirs(output_dir, exist_ok=True)

# Resize for different icon sizes
sizes = [(256, 256), (128, 128), (64, 64), (48, 48), (32, 32), (16, 16)]
icons = [img.resize(s, Image.Resampling.LANCZOS) for s in sizes]

ico_path = os.path.join(output_dir, 'app.ico')
icons[0].save(ico_path, format='ICO', sizes=sizes)

print(f"Icon created: {ico_path}")
