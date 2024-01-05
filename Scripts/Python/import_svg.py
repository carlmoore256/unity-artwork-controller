import os
from bs4 import BeautifulSoup
import os
import json
from svgpathtools.parser import parse_path
import shutil
import argparse

UNITY_PROJECT_PATH = "./"
UNITY_SVG_PATH = os.path.join(UNITY_PROJECT_PATH, "Assets/SVG")

def calculate_offset(path_element, height, width):
    d = path_element['d']
    path = parse_path(d)
    bbox = path.bbox()
    # bbox is: xmin, xmax, ymin, ymax
    center = (bbox[0] + bbox[1]) / 2, (bbox[2] + bbox[3]) / 2
    viewbox_center = width / 2, height / 2
    offset = (viewbox_center[0] - center[0]) * -1, viewbox_center[1] - center[1]
    return offset

# creates an svg package that can be opened by Unity
def create_layered_svg_package(svg_path, artwork_name):
    # create the necessary directories
    if (os.path.exists(UNITY_SVG_PATH) == False):
        os.mkdir(UNITY_SVG_PATH)
    artwork_path = os.path.join(UNITY_SVG_PATH, artwork_name)
    if (os.path.exists(artwork_path) == False):
        os.mkdir(artwork_path)
    original_path = os.path.join(artwork_path, "Original")
    if (os.path.exists(original_path) == False):
        os.mkdir(original_path)
    layers_path = os.path.join(artwork_path, "Layers")
    if (os.path.exists(layers_path) == False):
        os.mkdir(layers_path)
    
    svg_original_path = os.path.join(original_path, "original.svg")
    # copy the original svg to the original folder
    shutil.copy(svg_path, svg_original_path)

    split_into_layers(svg_original_path, layers_path)

def split_into_layers(input_svg_path, output_root):
    with open(input_svg_path, "r") as f:
        svg_str = f.read()
    soup = BeautifulSoup(svg_str, "lxml")
    original_svg = soup.svg
    template_svg = soup.new_tag("svg")
    # Copy attributes from the original SVG tag to the new SVG tag
    for attr in original_svg.attrs:
        template_svg.attrs[attr] = original_svg.attrs[attr]
    template_svg = str(template_svg)
    height = int(soup.svg.attrs["height"])
    width = int(soup.svg.attrs["width"])
    offsets = []
    no_id_count = 0
    for i, group in enumerate(soup.find_all("g")):
        new_svg = BeautifulSoup(template_svg, "xml")
        new_svg.svg.append(group)
        id = group.attrs["id"]
        if id == "" or id is None:
            filename = f"layer-no-id-{no_id_count}.svg"
            no_id_count += 1
        else:
            filename = f"{id}.svg"
        outfile = os.path.join(output_root, filename)
        with open(outfile, "w") as f:
            f.write(str(new_svg))

        path = group.find("path")
        if path is None:
            print(f"Group {id} has no path")
            offset = (0, 0)
        else:
            offset = calculate_offset(group.find("path"), height, width)

        offsets.append({"file" : filename, "offset" : offset})
    with open(os.path.join(output_root, "layers.json"), "w") as f:
        f.write(json.dumps(offsets))
    print(f'Finished splitting {input_svg_path} into layers | Output: {output_root}')


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description='Create a layered SVG package for Unity')
    parser.add_argument('svg_path', type=str, help='Path to the SVG file')
    parser.add_argument('artwork_name', type=str, help='Name of the artwork')
    args = parser.parse_args()

    # artwork_name = "MagiciansVision"
    # original_path = "D:/MarioProject/test-image-only-paths.svg"
    create_layered_svg_package(args.svg_path, args.artwork_name)
