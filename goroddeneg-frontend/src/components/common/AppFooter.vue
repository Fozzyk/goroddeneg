<template>
  <footer class="footer">
    <div class="container">
      <div class="footer-grid">
        <div class="footer-brand">
          <div class="footer-logo">
            <div class="fl-icon">Г</div>
            <span>Город <strong>Денег</strong></span>
          </div>
          <p class="footer-desc">Краудфандинговая платформа Казахстана. Помогаем воплощать идеи с 2024 года.</p>
          <div class="footer-socials">
            <a href="#" class="social-link" title="Telegram">✈️</a>
            <a href="#" class="social-link" title="Instagram">📸</a>
            <a href="#" class="social-link" title="Facebook">📘</a>
          </div>
        </div>
        <div class="footer-col">
          <div class="fc-title">Платформа</div>
          <router-link to="/projects"     class="fc-link">Проекты</router-link>
          <router-link to="/how-it-works" class="fc-link">Как это работает</router-link>
          <router-link to="/create"       class="fc-link">Создать проект</router-link>
          <router-link to="/about"        class="fc-link">О нас</router-link>
          <router-link to="/why"          class="fc-link">Почему мы</router-link>
        </div>
        <div class="footer-col">
          <div class="fc-title">Правовое</div>
          <a href="#" class="fc-link">Политика конфиденциальности</a>
          <a href="#" class="fc-link">Пользовательское соглашение</a>
          <a href="#" class="fc-link">Тарифы</a>
          <a href="#" class="fc-link">Публичная оферта</a>
        </div>
        <div class="footer-col">
          <div class="fc-title">Контакты</div>
          <a href="mailto:hello@goroddeneg.kz" class="fc-link">📧 hello@goroddeneg.kz</a>
          <a href="tel:+77271234567"           class="fc-link">📞 +7 (727) 123-45-67</a>
          <span class="fc-link">📍 г. Алматы, ул. Абая, 10</span>
          <span class="fc-link" style="color:var(--text-light);font-size:12px">Пн–Пт: 9:00–18:00</span>
        </div>
      </div>
      <div class="footer-bottom">
        <div class="fb-copy">© {{ year }} ТОО «Город Денег». Все права защищены.</div>
        <div class="fb-made">Сделано в Казахстане 🇰🇿</div>
      </div>
    </div>
  </footer>
</template>
<script setup>
const year = new Date().getFullYear()
</script>
<style scoped>
.footer { background: var(--text); padding: 64px 0 0; margin-top: auto; }
.footer-grid { display: grid; grid-template-columns: 2fr 1fr 1fr 1fr; gap: 48px; padding-bottom: 48px; border-bottom: 1px solid rgba(255,255,255,.08); }
.footer-logo { display: flex; align-items: center; gap: 10px; margin-bottom: 14px; }
.fl-icon { width: 34px; height: 34px; border-radius: 10px; background: var(--blue); color: #fff; font-family: 'Bricolage Grotesque', sans-serif; font-size: 18px; font-weight: 800; display: flex; align-items: center; justify-content: center; }
.footer-logo span { font-family: 'Bricolage Grotesque', sans-serif; font-size: 17px; font-weight: 600; color: #fff; }
.footer-logo strong { color: var(--blue); font-weight: 800; }
.footer-desc { font-size: 13px; color: rgba(255,255,255,.45); line-height: 1.7; margin-bottom: 20px; max-width: 260px; }
.footer-socials { display: flex; gap: 10px; }
.social-link { width: 36px; height: 36px; border-radius: 50%; background: rgba(255,255,255,.07); border: 1px solid rgba(255,255,255,.1); display: flex; align-items: center; justify-content: center; font-size: 14px; text-decoration: none; transition: all .2s; }
.social-link:hover { background: rgba(27,127,225,.3); border-color: var(--blue); }
.fc-title { font-size: 11px; font-weight: 700; letter-spacing: .08em; text-transform: uppercase; color: rgba(255,255,255,.3); margin-bottom: 16px; }
.fc-link { display: block; font-size: 13px; color: rgba(255,255,255,.55); text-decoration: none; margin-bottom: 10px; transition: color .15s; cursor: pointer; }
.fc-link:hover { color: #fff; text-decoration: none; }
.footer-bottom { display: flex; justify-content: space-between; align-items: center; padding: 20px 0; }
.fb-copy, .fb-made { font-size: 12px; color: rgba(255,255,255,.25); }
@media (max-width: 900px) { .footer-grid { grid-template-columns: 1fr 1fr; gap: 32px; } .footer-brand { grid-column: 1/-1; } }
@media (max-width: 540px) { .footer-grid { grid-template-columns: 1fr; } .footer-bottom { flex-direction: column; gap: 8px; text-align: center; } }
</style>
