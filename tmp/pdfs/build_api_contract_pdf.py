from __future__ import annotations

import html
import re
from pathlib import Path

from reportlab.lib import colors
from reportlab.lib.enums import TA_CENTER, TA_LEFT
from reportlab.lib.pagesizes import A4
from reportlab.lib.styles import ParagraphStyle, getSampleStyleSheet
from reportlab.lib.units import mm
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont
from reportlab.platypus import (
    BaseDocTemplate,
    Frame,
    HRFlowable,
    KeepTogether,
    PageBreak,
    PageTemplate,
    Paragraph,
    Preformatted,
    Spacer,
    Table,
    TableStyle,
)


ROOT = Path(r"C:\Users\ashra\source\repos\TickitngSystem")
SOURCE = Path(__file__).with_name("API_CONTRACT.md")
OUTPUT = ROOT / "output" / "pdf" / "TaskFlow_API_Contract.pdf"

PAGE_WIDTH, PAGE_HEIGHT = A4
LEFT = 18 * mm
RIGHT = 18 * mm
TOP = 18 * mm
BOTTOM = 17 * mm
CONTENT_WIDTH = PAGE_WIDTH - LEFT - RIGHT

INK = colors.HexColor("#172033")
MUTED = colors.HexColor("#667085")
PURPLE = colors.HexColor("#6737D1")
PURPLE_LIGHT = colors.HexColor("#F1ECFF")
BORDER = colors.HexColor("#D9DDEA")
CODE_BG = colors.HexColor("#F5F6FA")
TABLE_HEAD = colors.HexColor("#ECE8F8")
WHITE = colors.white


def register_fonts() -> tuple[str, str, str]:
    candidates = [
        (
            Path(r"C:\Windows\Fonts\segoeui.ttf"),
            Path(r"C:\Windows\Fonts\segoeuib.ttf"),
            Path(r"C:\Windows\Fonts\consola.ttf"),
        ),
        (
            Path(r"C:\Windows\Fonts\arial.ttf"),
            Path(r"C:\Windows\Fonts\arialbd.ttf"),
            Path(r"C:\Windows\Fonts\cour.ttf"),
        ),
    ]
    for regular, bold, mono in candidates:
        if regular.exists() and bold.exists() and mono.exists():
            pdfmetrics.registerFont(TTFont("ContractRegular", str(regular)))
            pdfmetrics.registerFont(TTFont("ContractBold", str(bold)))
            pdfmetrics.registerFont(TTFont("ContractMono", str(mono)))
            return "ContractRegular", "ContractBold", "ContractMono"
    return "Helvetica", "Helvetica-Bold", "Courier"


REGULAR, BOLD, MONO = register_fonts()


def inline_markup(value: str) -> str:
    value = html.escape(value.strip())
    value = re.sub(r"`([^`]+)`", r'<font name="ContractMono" color="#4E2AA8">\1</font>', value)
    value = re.sub(r"\*\*([^*]+)\*\*", r"<b>\1</b>", value)
    return value


styles = getSampleStyleSheet()
styles.add(
    ParagraphStyle(
        name="ContractBody",
        fontName=REGULAR,
        fontSize=9.2,
        leading=13.2,
        textColor=INK,
        spaceAfter=5,
    )
)
styles.add(
    ParagraphStyle(
        name="ContractH1",
        fontName=BOLD,
        fontSize=23,
        leading=28,
        textColor=INK,
        spaceAfter=8,
    )
)
styles.add(
    ParagraphStyle(
        name="ContractH2",
        fontName=BOLD,
        fontSize=15,
        leading=19,
        textColor=PURPLE,
        spaceBefore=10,
        spaceAfter=6,
        keepWithNext=True,
    )
)
styles.add(
    ParagraphStyle(
        name="ContractH3",
        fontName=BOLD,
        fontSize=11.5,
        leading=15,
        textColor=INK,
        spaceBefore=7,
        spaceAfter=4,
        keepWithNext=True,
    )
)
styles.add(
    ParagraphStyle(
        name="ContractBullet",
        parent=styles["ContractBody"],
        leftIndent=12,
        firstLineIndent=-7,
        bulletIndent=0,
        spaceAfter=2.5,
    )
)
styles.add(
    ParagraphStyle(
        name="ContractCode",
        fontName=MONO,
        fontSize=7.7,
        leading=10.2,
        textColor=INK,
        leftIndent=7,
        rightIndent=7,
        borderColor=BORDER,
        borderWidth=0.6,
        borderPadding=7,
        backColor=CODE_BG,
        spaceBefore=3,
        spaceAfter=7,
    )
)
styles.add(
    ParagraphStyle(
        name="TableCell",
        fontName=REGULAR,
        fontSize=7.7,
        leading=10.2,
        textColor=INK,
    )
)
styles.add(
    ParagraphStyle(
        name="TableHead",
        fontName=BOLD,
        fontSize=7.7,
        leading=10.2,
        textColor=INK,
    )
)


class ContractDoc(BaseDocTemplate):
    def __init__(self, filename: str):
        super().__init__(
            filename,
            pagesize=A4,
            leftMargin=LEFT,
            rightMargin=RIGHT,
            topMargin=TOP,
            bottomMargin=BOTTOM,
            title="TaskFlow API Contract",
            author="TaskFlow",
            subject="Frontend integration contract for the TaskFlow API",
        )
        frame = Frame(LEFT, BOTTOM, CONTENT_WIDTH, PAGE_HEIGHT - TOP - BOTTOM, id="content")
        self.addPageTemplates(PageTemplate(id="main", frames=[frame], onPage=draw_page))


def draw_page(canvas, doc):
    canvas.saveState()
    canvas.setStrokeColor(BORDER)
    canvas.setLineWidth(0.5)
    canvas.line(LEFT, PAGE_HEIGHT - 11 * mm, PAGE_WIDTH - RIGHT, PAGE_HEIGHT - 11 * mm)
    canvas.setFont(BOLD, 8)
    canvas.setFillColor(PURPLE)
    canvas.drawString(LEFT, PAGE_HEIGHT - 8.2 * mm, "TaskFlow")
    canvas.setFont(REGULAR, 7.5)
    canvas.setFillColor(MUTED)
    canvas.drawRightString(PAGE_WIDTH - RIGHT, PAGE_HEIGHT - 8.2 * mm, "API Contract")
    canvas.line(LEFT, 11 * mm, PAGE_WIDTH - RIGHT, 11 * mm)
    canvas.drawString(LEFT, 7.2 * mm, "Generated from the backend source contract")
    canvas.drawRightString(PAGE_WIDTH - RIGHT, 7.2 * mm, f"Page {doc.page}")
    canvas.restoreState()


def parse_table(lines: list[str]) -> Table:
    rows: list[list[Paragraph]] = []
    for index, line in enumerate(lines):
        if index == 1:
            continue
        cells = [cell.strip() for cell in line.strip().strip("|").split("|")]
        style = styles["TableHead"] if index == 0 else styles["TableCell"]
        rows.append([Paragraph(inline_markup(cell), style) for cell in cells])

    columns = len(rows[0])
    if columns == 2:
        widths = [CONTENT_WIDTH * 0.29, CONTENT_WIDTH * 0.71]
    elif columns == 3:
        widths = [CONTENT_WIDTH * 0.16, CONTENT_WIDTH * 0.45, CONTENT_WIDTH * 0.39]
    elif columns == 4:
        widths = [CONTENT_WIDTH * 0.13, CONTENT_WIDTH * 0.43, CONTENT_WIDTH * 0.19, CONTENT_WIDTH * 0.25]
    else:
        widths = [CONTENT_WIDTH / columns] * columns

    table = Table(rows, colWidths=widths, repeatRows=1, hAlign="LEFT")
    table.setStyle(
        TableStyle(
            [
                ("BACKGROUND", (0, 0), (-1, 0), TABLE_HEAD),
                ("BOX", (0, 0), (-1, -1), 0.6, BORDER),
                ("INNERGRID", (0, 0), (-1, -1), 0.35, BORDER),
                ("VALIGN", (0, 0), (-1, -1), "TOP"),
                ("LEFTPADDING", (0, 0), (-1, -1), 5),
                ("RIGHTPADDING", (0, 0), (-1, -1), 5),
                ("TOPPADDING", (0, 0), (-1, -1), 5),
                ("BOTTOMPADDING", (0, 0), (-1, -1), 5),
                ("ROWBACKGROUNDS", (0, 1), (-1, -1), [WHITE, colors.HexColor("#FAFAFC")]),
            ]
        )
    )
    return table


def markdown_to_story(text: str):
    story = []
    lines = text.splitlines()
    index = 0
    first_heading = True

    while index < len(lines):
        line = lines[index].rstrip()

        if not line:
            index += 1
            continue

        if line.startswith("```"):
            code_lines = []
            index += 1
            while index < len(lines) and not lines[index].startswith("```"):
                code_lines.append(lines[index])
                index += 1
            code = "\n".join(code_lines)
            story.append(Preformatted(code, styles["ContractCode"], maxLineLength=92))
            index += 1
            continue

        if line.startswith("|") and index + 1 < len(lines) and re.match(r"^\|?\s*:?-+", lines[index + 1]):
            table_lines = [line, lines[index + 1]]
            index += 2
            while index < len(lines) and lines[index].strip().startswith("|"):
                table_lines.append(lines[index])
                index += 1
            story.append(parse_table(table_lines))
            story.append(Spacer(1, 7))
            continue

        if line.startswith("# "):
            if first_heading:
                story.extend(
                    [
                        Spacer(1, 8 * mm),
                        Paragraph(inline_markup(line[2:]), styles["ContractH1"]),
                        HRFlowable(width="100%", thickness=2, color=PURPLE, spaceAfter=8),
                        Paragraph(
                            "Frontend integration reference for authentication, accounts, employees, projects, tickets, workflows, and attachments.",
                            ParagraphStyle(
                                "Subtitle",
                                parent=styles["ContractBody"],
                                fontSize=11,
                                leading=16,
                                textColor=MUTED,
                                spaceAfter=8,
                            ),
                        ),
                        Paragraph("Version 1.0 | September 2026", styles["ContractBody"]),
                        Spacer(1, 8 * mm),
                    ]
                )
                first_heading = False
            else:
                story.append(PageBreak())
                story.append(Paragraph(inline_markup(line[2:]), styles["ContractH1"]))
            index += 1
            continue

        if line.startswith("## "):
            story.append(Paragraph(inline_markup(line[3:]), styles["ContractH2"]))
            index += 1
            continue

        if line.startswith("### "):
            story.append(Paragraph(inline_markup(line[4:]), styles["ContractH3"]))
            index += 1
            continue

        if line.startswith("- "):
            story.append(Paragraph(inline_markup(line[2:]), styles["ContractBullet"], bulletText="-"))
            index += 1
            continue

        paragraph_lines = [line]
        index += 1
        while index < len(lines):
            next_line = lines[index].rstrip()
            if (
                not next_line
                or next_line.startswith("#")
                or next_line.startswith("- ")
                or next_line.startswith("```")
                or next_line.startswith("|")
            ):
                break
            paragraph_lines.append(next_line)
            index += 1
        story.append(Paragraph(inline_markup(" ".join(paragraph_lines)), styles["ContractBody"]))

    return story


def main():
    OUTPUT.parent.mkdir(parents=True, exist_ok=True)
    source_text = SOURCE.read_text(encoding="utf-8")
    story = markdown_to_story(source_text)
    doc = ContractDoc(str(OUTPUT))
    doc.build(story)
    print(OUTPUT)


if __name__ == "__main__":
    main()
