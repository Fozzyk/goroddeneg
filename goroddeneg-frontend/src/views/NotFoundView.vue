<template>
  <div class="notfound-page">
    <div class="nf-content">
      <div class="nf-code font-display">404</div>
      <h1 class="nf-title">Страница не найдена</h1>
      <p class="text-muted nf-desc">
        Похоже, эта страница переехала или никогда не существовала.<br/>
        Но у нас есть много интересных проектов!
      </p>
      <div class="nf-actions">
        <router-link to="/"        class="btn btn-primary btn-lg">🏠 На главную</router-link>
        <router-link to="/projects" class="btn btn-outline  btn-lg">📂 Все проекты</router-link>
      </div>
    </div>
    <div class="nf-art">
      <div class="nf-circle c1" />
      <div class="nf-circle c2" />
      <div class="nf-circle c3" />
    </div>
  </div>
</template>

<style scoped>
.notfound-page {
  min-height: 80vh; display: flex; align-items: center; justify-content: center;
  position: relative; overflow: hidden; padding: 40px 24px;
}
.nf-content { text-align: center; position: relative; z-index: 1; }
.nf-code    {
  font-size: clamp(6rem, 18vw, 12rem); font-weight: 900; line-height: 1;
  color: var(--gold-pale); letter-spacing: -0.04em; margin-bottom: 8px;
}
.nf-title   { font-size: 2rem; margin-bottom: 16px; }
.nf-desc    { font-size: 16px; line-height: 1.7; max-width: 400px; margin: 0 auto 36px; }
.nf-actions { display: flex; gap: 14px; justify-content: center; flex-wrap: wrap; }

.nf-art     { position: absolute; inset: 0; pointer-events: none; }
.nf-circle  {
  position: absolute; border-radius: 50%;
  background: radial-gradient(circle, rgba(166,124,0,.08) 0%, transparent 70%);
}
.c1 { width: 600px; height: 600px; top: -200px; right: -200px; }
.c2 { width: 400px; height: 400px; bottom: -150px; left: -100px; }
.c3 { width: 200px; height: 200px; top: 40%; left: 10%; }
</style>
