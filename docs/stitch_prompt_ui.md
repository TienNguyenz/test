Build a web-based admin dashboard for a distributed retail database demo where SQL Server is HQ and MySQL is Branch. The page should feel bold, data-centric, and premium, not generic bootstrap.

**DESIGN SYSTEM (REQUIRED):**
- Platform: Web, desktop-first with mobile adaptation
- Theme: Light editorial tech look with high contrast and strong hierarchy
- Typography: Space Grotesk for headings, Plus Jakarta Sans for body text
- Background: Layered warm-gray gradient (#f4f2ee to #ebe7de) with subtle geometric glow shapes
- Primary Accent: Deep Teal (#0b5d6e) for CTAs and active tabs
- Secondary Accent: Burnt Orange (#d97706) for highlights and badges
- Surface: Ivory (#fffdf8) cards with soft blur and thin border
- Text Primary: Charcoal (#1e293b)
- Text Secondary: Slate (#64748b)
- Border Radius: 14px cards, 10px inputs/buttons
- Elevation: Soft shadow for cards; stronger shadow on hover
- Motion: Staggered card reveal and slight lift on hover

**Page Structure:**
1. **Top Command Bar:** Product name, environment badges (SQL Server HQ, MySQL Branch), quick actions.
2. **Integration Health Strip:** Linked server status, HQ product count, branch invoice count, queue success/error counters.
3. **Transparent JOIN Section:** SQL snippet block + table showing joined data from HQ SanPham and Branch HoaDon.
4. **Product CRUD Workspace:**
   - Product list table with Add/Edit/Delete actions.
   - Right-side form panel with validated fields (ProductName, Price, Category).
   - Inline validation and constraint helper text.
5. **Price Sync Timeline:** Queue events from Pending to Success/Error with timestamps.
6. **Report Section:** Revenue summary cards and filter controls (from/to date).

**UX Requirements:**
- Show explicit distributed-data evidence labels on each section.
- Keep SQL-related terminology visible for academic demo context.
- Make forms and tables dense enough for admin workflows but still visually polished.
- Avoid dark mode; keep a warm light enterprise aesthetic.

---
Tip: For consistent multi-page results, create a DESIGN.md and reuse this token system for every generated screen.
