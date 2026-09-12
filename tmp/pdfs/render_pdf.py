from pathlib import Path

import pypdfium2 as pdfium
from PIL import Image, ImageDraw


pdf_path = Path(r"C:\Users\ashra\source\repos\TickitngSystem\output\pdf\TaskFlow_API_Contract.pdf")
render_dir = Path(r"C:\Users\ashra\source\repos\TickitngSystem\tmp\pdfs\rendered")
render_dir.mkdir(parents=True, exist_ok=True)

document = pdfium.PdfDocument(str(pdf_path))
rendered = []
for index, page in enumerate(document):
    image = page.render(scale=1.5).to_pil().convert("RGB")
    page_path = render_dir / f"page-{index + 1:02d}.png"
    image.save(page_path)
    rendered.append(image)

thumb_width = 420
gap = 18
label_height = 28
thumbs = []
for index, image in enumerate(rendered):
    ratio = thumb_width / image.width
    thumb = image.resize((thumb_width, int(image.height * ratio)))
    card = Image.new("RGB", (thumb_width, thumb.height + label_height), "white")
    card.paste(thumb, (0, label_height))
    ImageDraw.Draw(card).text((8, 6), f"Page {index + 1}", fill="#172033")
    thumbs.append(card)

columns = 2
rows = (len(thumbs) + columns - 1) // columns
cell_height = max(image.height for image in thumbs)
sheet = Image.new(
    "RGB",
    (columns * thumb_width + (columns + 1) * gap, rows * cell_height + (rows + 1) * gap),
    "#d9ddea",
)
for index, image in enumerate(thumbs):
    x = gap + (index % columns) * (thumb_width + gap)
    y = gap + (index // columns) * (cell_height + gap)
    sheet.paste(image, (x, y))

sheet.save(render_dir / "contact-sheet.png")
print(render_dir / "contact-sheet.png")
