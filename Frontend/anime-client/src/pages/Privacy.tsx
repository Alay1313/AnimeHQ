export function Privacy() {
  return (
    <div className="container mx-auto px-4 py-12 max-w-3xl space-y-8">
      <div>
        <h1 className="text-3xl font-bold tracking-tight">Privacy Policy</h1>
        <p className="text-muted-foreground mt-2">Last updated: {new Date().toLocaleDateString()}</p>
      </div>

      <div className="space-y-6 text-sm leading-relaxed text-muted-foreground">
        <section className="space-y-2">
          <h2 className="text-lg font-semibold text-foreground">Overview</h2>
          <p>
            Anime Harbour is a personal portfolio project built to demonstrate full-stack
            development skills. This page explains what information is collected and how it's used.
          </p>
        </section>

        <section className="space-y-2">
          <h2 className="text-lg font-semibold text-foreground">Information We Collect</h2>
          <p>When you create an account, we store:</p>
          <ul className="list-disc list-inside space-y-1 ml-2">
            <li>Your username and email address</li>
            <li>A securely hashed version of your password (we never store plain-text passwords)</li>
            <li>Your favorites, reviews, and ratings within the app</li>
          </ul>
        </section>

        <section className="space-y-2">
          <h2 className="text-lg font-semibold text-foreground">How We Use Your Information</h2>
          <p>
            Your account information is used solely to provide app functionality — letting you
            log in, save favorites, and leave reviews. We do not sell, rent, or share your data
            with third parties.
          </p>
        </section>

        <section className="space-y-2">
          <h2 className="text-lg font-semibold text-foreground">Third-Party Services</h2>
          <p>
            Anime and manga data is sourced from the{" "}
            
              href="https://jikan.moe"
              target="_blank"
              rel="noopener noreferrer"
              className="underline hover:text-foreground transition-colors"
            <a>
              Jikan API
            </a>
            , an unofficial MyAnimeList API. No personal account information is sent to Jikan —
            only anime/manga search queries.
          </p>
        </section>

        <section className="space-y-2">
          <h2 className="text-lg font-semibold text-foreground">Cookies & Tracking</h2>
          <p>
            This site does not use advertising cookies or third-party tracking scripts. A
            authentication token is stored in your browser's local storage to keep you signed in.
          </p>
        </section>

        <section className="space-y-2">
          <h2 className="text-lg font-semibold text-foreground">Data Deletion</h2>
          <p>
            Since this is a portfolio project, account and data deletion requests can be made by
            contacting the site owner directly via the email link in the footer.
          </p>
        </section>

        <section className="space-y-2">
          <h2 className="text-lg font-semibold text-foreground">Changes to This Policy</h2>
          <p>
            This policy may be updated as the project evolves. Continued use of the site after
            changes constitutes acceptance of the updated policy.
          </p>
        </section>
      </div>
    </div>
  );
}