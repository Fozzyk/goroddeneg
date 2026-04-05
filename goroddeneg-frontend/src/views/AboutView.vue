<!-- AboutView.vue -->
<template>
  <div class="page">
    <div class="container" style="max-width:960px">
      <div class="about-hero">
        <span class="badge badge-gold mb-4">О платформе</span>
        <h1>«Город Денег» —<br/>пространство для идей</h1>
        <p class="about-lead text-muted">
          Мы создаём пространство, в котором каждый может поделиться своей уникальной задумкой и получить необходимую поддержку для её реализации.
        </p>
      </div>

      <div class="about-story card card-body">
        <div class="story-grid">
          <div>
            <h2 class="font-display mb-4">Наша история</h2>
            <p class="text-muted mb-4">«Город денег» был основан в 2024 году группой предпринимателей из Алматы, которые верили в силу сообщества и коллективного финансирования. Мы видели, как многие талантливые люди не могли воплотить свои идеи в жизнь из-за отсутствия начального капитала.</p>
            <p class="text-muted">Сегодня наша платформа объединяет изобретателей, новаторов и всех, кто стремится к переменам — от технических стартапов до творческих инициатив.</p>
          </div>
          <div class="story-stats">
            <div v-for="s in stats" :key="s.label" class="story-stat">
              <div class="ss-icon">{{ s.icon }}</div>
              <div class="ss-val font-display">{{ s.val }}</div>
              <div class="ss-lbl text-muted">{{ s.label }}</div>
            </div>
          </div>
        </div>
      </div>

      <div class="values-grid mt-8">
        <div v-for="v in values" :key="v.title" class="value-card card card-body">
          <div style="font-size:36px;margin-bottom:16px">{{ v.icon }}</div>
          <h3>{{ v.title }}</h3>
          <p class="text-muted mt-2">{{ v.text }}</p>
        </div>
      </div>

      <div class="team-section mt-8">
        <h2 class="font-display mb-6 text-center">Наша команда</h2>
        <div class="team-grid">
          <div v-for="m in team" :key="m.name" class="team-card card card-body">
            <div class="tm-avatar">{{ m.initials }}</div>
            <div class="tm-name">{{ m.name }}</div>
            <div class="tm-role text-muted">{{ m.role }}</div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
<script setup>
const stats = [
  { icon:'📅', val:'2024', label:'Год основания' },
  { icon:'🏙️', val:'Алматы', label:'Штаб-квартира' },
  { icon:'📂', val:'1 240+', label:'Проектов запущено' },
  { icon:'👥', val:'45 000+', label:'Пользователей' },
]
const values = [
  { icon:'🔍', title:'Прозрачность', text:'Все процессы финансирования открыты. Создатели и спонсоры всегда в курсе статуса проекта.' },
  { icon:'🤝', title:'Доверие', text:'Мы тщательно проверяем каждый проект перед публикацией, обеспечивая безопасность для всех сторон.' },
  { icon:'💡', title:'Инновации', text:'Мы постоянно развиваем платформу, внедряя новые инструменты для создателей и спонсоров.' },
  { icon:'❤️', title:'Сообщество', text:'За каждым проектом стоит сообщество людей, объединённых общей целью сделать мир лучше.' },
]
const team = [
  { name:'Айбек Джаксыбеков', role:'CEO & Co-founder',    initials:'АД' },
  { name:'Динара Сейткали',   role:'CTO & Co-founder',    initials:'ДС' },
  { name:'Марат Нурланов',    role:'Head of Product',     initials:'МН' },
  { name:'Айгуль Бекова',     role:'Head of Marketing',   initials:'АБ' },
]
</script>
<style scoped>
.about-hero { text-align: center; margin-bottom: 48px; }
.about-lead { font-size: 17px; line-height: 1.75; max-width: 600px; margin: 16px auto 0; }
.story-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 48px; align-items: center; }
.story-stats { display: grid; grid-template-columns: 1fr 1fr; gap: 16px; }
.story-stat  { background: var(--gold-subtle); border: 1px solid var(--border); border-radius: var(--r-md); padding: 20px; text-align: center; }
.ss-icon { font-size: 28px; margin-bottom: 10px; }
.ss-val  { font-size: 1.5rem; color: var(--gold); }
.ss-lbl  { font-size: 12px; margin-top: 4px; }
.values-grid { display: grid; grid-template-columns: repeat(4,1fr); gap: 20px; }
.value-card h3 { font-size: 1.05rem; }
.team-grid  { display: grid; grid-template-columns: repeat(4,1fr); gap: 20px; }
.team-card  { text-align: center; }
.tm-avatar  { width: 72px; height: 72px; border-radius: 50%; background: linear-gradient(135deg, var(--gold), var(--gold-light)); color: #fff; font-size: 24px; font-weight: 700; display: flex; align-items: center; justify-content: center; margin: 0 auto 14px; }
.tm-name { font-weight: 700; margin-bottom: 4px; }
.tm-role { font-size: 13px; }
@media (max-width: 900px) {
  .story-grid  { grid-template-columns: 1fr; }
  .values-grid { grid-template-columns: 1fr 1fr; }
  .team-grid   { grid-template-columns: 1fr 1fr; }
}
</style>
