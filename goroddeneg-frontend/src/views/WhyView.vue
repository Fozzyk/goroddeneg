<template>
  <div class="page">
    <div class="container" style="max-width:900px">
      <div class="why-header">
        <span class="badge badge-gold mb-4">Преимущества</span>
        <h1>Почему это выгодно?</h1>
        <p class="text-muted mt-2">Двойная победа — для создателей и для спонсоров</p>
      </div>

      <div class="why-tabs">
        <button v-for="tab in tabs" :key="tab" class="why-tab" :class="{ active: active === tab }" @click="active = tab">{{ tab }}</button>
      </div>

      <div class="why-items">
        <div v-for="(item, i) in currentItems" :key="i" class="why-item card card-body">
          <div class="wi-num">{{ String(i+1).padStart(2,'0') }}</div>
          <div class="wi-icon">{{ item.icon }}</div>
          <div class="wi-body">
            <h3>{{ item.title }}</h3>
            <p class="text-muted mt-2">{{ item.text }}</p>
          </div>
        </div>
      </div>

      <div class="why-cta card card-body">
        <h2 class="font-display">Присоединяйтесь к «Городу денег»</h2>
        <p class="text-muted mt-2">Вместе строим будущее, где каждая мечта имеет право на жизнь</p>
        <div class="flex gap-4 mt-6" style="justify-content:center;flex-wrap:wrap">
          <router-link to="/create"   class="btn btn-primary btn-lg">Создать проект</router-link>
          <router-link to="/projects" class="btn btn-outline btn-lg">Поддержать проект</router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
const tabs = ['Для стартапа', 'Для инвестора']
const active = ref('Для стартапа')

const data = {
  'Для стартапа': [
    { icon:'💸', title:'Доступ к финансированию', text:'Привлекайте средства без банковских кредитов, минуя традиционных инвесторов. Получайте финансирование напрямую от сообщества, которому нравится ваша идея.' },
    { icon:'📣', title:'Поддержка и обратная связь', text:'Получайте ценные отзывы от целевой аудитории ещё до начала производства. Это поможет улучшить продукт и повысить шансы на успех.' },
    { icon:'🤝', title:'Построение сообщества', text:'Привлекайте единомышленников и потенциальных клиентов, превращая их в первых сторонников вашего дела. Это не только финансирование, но и долгосрочные отношения.' },
    { icon:'📢', title:'Увеличение видимости', text:'Платформа обеспечивает широкий охват и продвижение. Чем больше людей узнают о вашем проекте, тем выше шансы на успех.' },
    { icon:'🎮', title:'Гибкость и контроль', text:'Вы сами управляете своим проектом. Мы предлагаем инструменты и поддержку, но вы принимаете все ключевые решения.' },
    { icon:'🛡️', title:'Минимальные риски', text:'Запускаясь на платформе, вы проверяете спрос до полной реализации. Предварительное финансирование подтверждает жизнеспособность идеи.' },
    { icon:'🔍', title:'Прозрачность и надёжность', text:'Все процессы финансирования открыты. Вы и ваши спонсоры всегда знаете, на каком этапе находится проект.' },
  ],
  'Для инвестора': [
    { icon:'🔍', title:'Прозрачность вложений', text:'Вы всегда видите, куда идут ваши деньги. Полная отчётность, история операций и регулярные обновления от создателей.' },
    { icon:'🌍', title:'Разнообразие проектов', text:'Сотни идей в разных категориях — от технологий до искусства. Поддерживайте то, что вам близко и в что вы верите.' },
    { icon:'🔒', title:'Безопасные транзакции', text:'Все платежи защищены. Если проект не собрал нужную сумму — деньги автоматически возвращаются на ваш баланс.' },
    { icon:'❤️', title:'Влияние на сообщество', text:'Ваша поддержка помогает создавать продукты и идеи, которые меняют жизнь вокруг.' },
    { icon:'🎁', title:'Эксклюзивные вознаграждения', text:'Спонсоры получают уникальные вознаграждения — первые экземпляры, персональные благодарности и эксклюзивный доступ.' },
    { icon:'💬', title:'Прямой контакт с создателями', text:'Общайтесь напрямую с авторами проектов, задавайте вопросы и следите за прогрессом в реальном времени.' },
  ]
}
const currentItems = computed(() => data[active.value])
</script>

<style scoped>
.why-header { text-align: center; margin-bottom: 40px; }
.why-tabs   { display: flex; justify-content: center; gap: 12px; margin-bottom: 40px; }
.why-tab {
  padding: 10px 28px;
  border-radius: var(--r-full);
  border: 2px solid var(--border);
  background: var(--bg);
  font-size: 15px;
  font-weight: 600;
  cursor: pointer;
  color: var(--text-mid);
  transition: all .18s;
  font-family: 'Inter', sans-serif;
  box-shadow: var(--shadow-xs);
}
.why-tab:hover:not(.active) {
  border-color: var(--blue);
  color: var(--blue);
  background: var(--blue-light);
}
.why-tab.active {
  background: var(--blue);
  color: #fff;
  border-color: var(--blue);
  box-shadow: var(--shadow-blue);
}
.why-tab:not(.active):hover { border-color: var(--gold); color: var(--gold); }
.why-items  { display: flex; flex-direction: column; gap: 14px; margin-bottom: 48px; }
.why-item   { display: flex; gap: 24px; align-items: flex-start; }
.wi-num  { font-family: 'Cormorant Garamond', serif; font-size: 2rem; font-weight: 700; color: var(--gold-pale); line-height: 1; width: 48px; flex-shrink: 0; text-align: right; }
.wi-icon { font-size: 30px; flex-shrink: 0; }
.wi-body h3 { font-size: 1.1rem; }
.why-cta { text-align: center; padding: 56px 40px; background: linear-gradient(135deg, var(--gold-subtle), var(--gold-pale)); }
</style>
