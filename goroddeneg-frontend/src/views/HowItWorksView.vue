<!-- ───────────────────────────────────────────────────────────
     HowItWorksView.vue
─────────────────────────────────────────────────────────── -->
<template>
  <div class="page">
    <div class="container" style="max-width:820px">
      <div class="info-header">
        <span class="badge badge-gold mb-4">Руководство</span>
        <h1>Как это работает</h1>
        <p class="text-muted mt-2">Пошаговый путь от идеи до успешного проекта</p>
      </div>
      <div class="steps-list">
        <div v-for="(s, i) in steps" :key="i" class="step-row card card-body">
          <div class="sr-num">{{ String(i+1).padStart(2,'0') }}</div>
          <div class="sr-icon">{{ s.icon }}</div>
          <div class="sr-body">
            <h3>{{ s.title }}</h3>
            <p class="text-muted mt-2">{{ s.text }}</p>
          </div>
        </div>
      </div>
      <div class="how-cta card card-body">
        <h2 class="font-display">Готовы начать?</h2>
        <p class="text-muted mt-2">Создайте проект за 15 минут и запустите сбор средств</p>
        <router-link to="/create" class="btn btn-primary btn-lg mt-4">Создать проект →</router-link>
      </div>
    </div>
  </div>
</template>
<script setup>
const steps = [
  { icon:'📋', title:'Регистрация и создание проекта', text:'Зарегистрируйтесь на платформе и создайте страницу проекта, где вы подробно опишете идею, цели и необходимую сумму финансирования. Добавьте визуальные материалы для лучшего восприятия.' },
  { icon:'🎁', title:'Представьте вознаграждения', text:'Определите уникальные вознаграждения для ваших спонсоров. Это могут быть первые экземпляры продукта, фирменные сувениры или эксклюзивные предложения. Чем интереснее — тем больше людей захотят поддержать вас.' },
  { icon:'📣', title:'Публикация и продвижение', text:'После создания запустите проект на платформе. Используйте встроенные инструменты для продвижения в социальных сетях. Регулярно делитесь обновлениями о процессе работы с сообществом.' },
  { icon:'💰', title:'Привлечение финансирования', text:'Платформа привлекает внимание потенциальных спонсоров. Получайте финансирование в реальном времени и взаимодействуйте с теми, кто проявляет интерес к вашей идее.' },
  { icon:'💬', title:'Взаимодействие и обновления', text:'Регулярно общайтесь с вашими спонсорами, отвечайте на вопросы и делитесь новостями о ходе реализации. Это поможет поддерживать высокий уровень доверия и заинтересованности.' },
  { icon:'🏆', title:'Получение средств и реализация', text:'После достижения цели вы получаете доступ к средствам и начинаете реализацию проекта. Длительное сотрудничество с вашими спонсорами открывает двери для дальнейших инициатив.' },
  { icon:'📊', title:'Отчёты и благодарности', text:'По завершении предоставьте своим спонсорам отчёты о достигнутых результатах и выразите благодарность. Это поможет установить долгосрочные отношения и поддержать ваши будущие начинания.' },
]
</script>
<style scoped>
.info-header { text-align: center; margin-bottom: 48px; }
.steps-list  { display: flex; flex-direction: column; gap: 16px; margin-bottom: 48px; }
.step-row    { display: flex; gap: 24px; align-items: flex-start; }
.sr-num  { font-family: 'Cormorant Garamond', serif; font-size: 2.4rem; font-weight: 700; color: var(--gold-pale); line-height: 1; width: 56px; flex-shrink: 0; text-align: right; }
.sr-icon { font-size: 32px; flex-shrink: 0; }
.sr-body h3  { font-size: 1.1rem; }
.how-cta { text-align: center; padding: 48px; background: linear-gradient(135deg, var(--gold-subtle), var(--gold-pale)); }
</style>
