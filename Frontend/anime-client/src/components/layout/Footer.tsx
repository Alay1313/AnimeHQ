import { Link } from "react-router-dom";

export function Footer() {
  return (
    <footer className="border-t mt-12">
      <div className="container mx-auto px-4 max-w-screen-2xl py-10">
        <div className="grid grid-cols-1 sm:grid-cols-3 gap-8">

          <div className="space-y-2">
            <div className="flex items-center space-x-2 font-bold text-lg tracking-tight">
              <span className="text-primary">Anime</span>
              <span>Harbour</span>
            </div>
            <p className="text-sm text-muted-foreground max-w-xs">
              A full-stack anime &amp; manga tracker built with React, .NET, and the Jikan API.
            </p>
          </div>

          <div className="space-y-2">
            <h3 className="text-sm font-semibold">Site</h3>
            <nav className="flex flex-col gap-2 text-sm text-muted-foreground">
              <Link to="/" className="hover:text-foreground transition-colors">Browse</Link>
              <Link to="/manga" className="hover:text-foreground transition-colors">Manga</Link>
              <Link to="/privacy" className="hover:text-foreground transition-colors">Privacy Policy</Link>
            </nav>
          </div>

          <div className="space-y-2">
            <h3 className="text-sm font-semibold">Connect</h3>
            <div className="flex items-center gap-3">
              <a href="https://github.com/Alay1313" target="_blank" rel="noopener noreferrer" className="text-muted-foreground hover:text-foreground transition-colors" aria-label="GitHub">
                <svg viewBox="0 0 24 24" className="h-5 w-5" fill="currentColor"><path d="M12 0C5.37 0 0 5.37 0 12c0 5.3 3.438 9.8 8.205 11.387.6.113.82-.258.82-.577 0-.285-.01-1.04-.015-2.04-3.338.724-4.042-1.61-4.042-1.61-.546-1.387-1.333-1.756-1.333-1.756-1.089-.745.083-.729.083-.729 1.205.084 1.84 1.237 1.84 1.237 1.07 1.834 2.807 1.304 3.492.997.108-.775.42-1.305.762-1.605-2.665-.303-5.467-1.332-5.467-5.93 0-1.31.469-2.381 1.236-3.221-.124-.303-.535-1.524.117-3.176 0 0 1.008-.322 3.3 1.23.957-.266 1.983-.399 3.003-.404 1.02.005 2.047.138 3.006.404 2.29-1.552 3.297-1.23 3.297-1.23.653 1.652.242 2.873.118 3.176.77.84 1.235 1.911 1.235 3.221 0 4.61-2.807 5.624-5.479 5.921.43.372.823 1.103.823 2.222 0 1.604-.015 2.896-.015 3.286 0 .322.216.696.825.578C20.565 21.795 24 17.298 24 12c0-6.63-5.373-12-12-12z"/></svg>
              </a>
              <a href="mailto:artjj09@gmail.com" className="text-muted-foreground hover:text-foreground transition-colors" aria-label="Email">
                <svg viewBox="0 0 24 24" className="h-5 w-5" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><path d="M22 6c0-1.1-.9-2-2-2H4c-1.1 0-2 .9-2 2v12c0 1.1.9 2 2 2h16c1.1 0 2-.9 2-2V6zm-2 0l-8 5-8-5h16zm0 12H4V8l8 5 8-5v10z"/></svg>
              </a>
            </div>
          </div>

        </div>

        <div className="border-t mt-8 pt-6 flex flex-col sm:flex-row items-center justify-between gap-3 text-xs text-muted-foreground">
          <p>&copy; {new Date().getFullYear()} Alie Shbur.</p>
          <p>
            Anime &amp; manga data provided by{" "}
            <a href="https://jikan.moe" target="_blank" rel="noopener noreferrer" className="underline hover:text-foreground transition-colors">Jikan API</a>
          </p>
        </div>
      </div>
    </footer>
  );
}