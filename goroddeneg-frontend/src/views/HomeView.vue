<template>
  <div>
    <!-- ── HERO ─────────────────────────────────── -->
    <section class="hero">
      <div class="container hero-inner">
        <div class="hero-content">
          <div class="hero-tag">🇰🇿 Краудфандинг в Казахстане</div>
          <h1 class="hero-title">
            Воплощайте идеи<br/>
            <span class="hero-blue">с поддержкой людей</span>
          </h1>
          <p class="hero-desc">
            «Город денег» — платформа, где авторы находят финансирование, а спонсоры поддерживают проекты, которые меняют жизнь вокруг.
          </p>
          <div class="hero-btns">
            <router-link to="/create"   class="btn btn-primary btn-lg">Запустить проект</router-link>
            <router-link to="/projects" class="btn btn-outline  btn-lg">Смотреть проекты</router-link>
          </div>
          <div class="hero-stats">
            <div class="hs-item">
              <div class="hs-num">1 240</div>
              <div class="hs-lbl">проектов запущено</div>
            </div>
            <div class="hs-sep" />
            <div class="hs-item">
              <div class="hs-num">890M ₸</div>
              <div class="hs-lbl">привлечено средств</div>
            </div>
            <div class="hs-sep" />
            <div class="hs-item">
              <div class="hs-num">45K</div>
              <div class="hs-lbl">активных спонсоров</div>
            </div>
          </div>
        </div>
        <div class="hero-cards" v-if="featured.length">
          <div class="hc-grid">
            <div v-for="p in featured" :key="p.id" class="hc-card" @click="$router.push(`/projects/${p.id}`)">
              <div class="hc-top">
                <span class="hc-cat">{{ p.categoryIcon }} {{ p.categoryName }}</span>
                <span class="hc-pct" :style="{ color: p.percentFunded>=100 ? 'var(--green)' : 'var(--blue)' }">
                  {{ Math.min(100,p.percentFunded) }}%
                </span>
              </div>
              <div class="hc-title">{{ truncate(p.title, 42) }}</div>
              <div class="progress progress-sm">
                <div class="progress-fill" :class="{green:p.percentFunded>=100}" :style="{ width: Math.min(100,p.percentFunded)+'%' }" />
              </div>
              <div class="hc-amount">{{ fmtM(p.collectedAmount) }}</div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- ── FEATURED ──────────────────────────────── -->
    <section class="section" style="background:var(--bg-soft)">
      <div class="container">
        <div class="sec-head">
          <div>
            <div class="section-tag">Сейчас в тренде</div>
            <h2>Актуальные проекты</h2>
          </div>
          <router-link to="/projects" class="btn btn-outline">Все проекты →</router-link>
        </div>
        <div v-if="loading" class="grid-4">
          <div v-for="i in 4" :key="i" class="skeleton-card" style="height:360px" />
        </div>
        <div v-else class="grid-4">
          <ProjectCard v-for="p in featured" :key="p.id" :project="p" />
        </div>
      </div>
    </section>

    <!-- ── HOW IT WORKS ───────────────────────────── -->
    <section class="section">
      <div class="container">
        <div class="text-center mb-8">
          <div class="section-tag">Просто и понятно</div>
          <h2>Как это работает</h2>
          <p style="margin-top:12px;max-width:520px;margin-left:auto;margin-right:auto">
            От идеи до финансирования за 4 простых шага
          </p>
        </div>
        <div class="how-grid">
          <div v-for="(s,i) in steps" :key="i" class="how-card">
            <div class="how-num">{{ String(i+1).padStart(2,'0') }}</div>
            <div class="how-icon">{{ s.icon }}</div>
            <h3 class="how-title">{{ s.title }}</h3>
            <p style="font-size:14px;color:var(--text-muted);margin-top:8px;line-height:1.6">{{ s.text }}</p>
          </div>
        </div>
        <div class="text-center mt-8">
          <router-link to="/how-it-works" class="btn btn-outline">Подробнее о процессе →</router-link>
        </div>
      </div>
    </section>

    <!-- ── WHY ───────────────────────────────────── -->
    <section class="section" style="background:var(--bg-soft)">
      <div class="container">
        <div class="text-center mb-8">
          <div class="section-tag">Преимущества</div>
          <h2>Почему выбирают нас</h2>
        </div>
        <div class="why-tabs">
          <button v-for="tab in ['Для создателей','Для спонсоров']" :key="tab"
            class="why-tab" :class="{active:activeTab===tab}" @click="activeTab=tab">{{ tab }}</button>
        </div>
        <div class="grid-2">
          <div v-for="w in whyItems" :key="w.title" class="why-card card card-body">
            <div class="why-icon">{{ w.icon }}</div>
            <h3 style="margin:12px 0 8px">{{ w.title }}</h3>
            <p style="font-size:14px;color:var(--text-muted);line-height:1.65">{{ w.text }}</p>
          </div>
        </div>
      </div>
    </section>

    <!-- ── CATEGORIES ─────────────────────────────── -->
    <section class="section">
      <div class="container">
        <div class="text-center mb-8">
          <div class="section-tag">Категории</div>
          <h2>Найдите свой проект</h2>
        </div>
        <div class="cats-grid">
          <router-link
            v-for="c in cats" :key="c.id"
            :to="`/projects?categoryId=${c.id}`"
            class="cat-pill"
          >{{ c.icon }} {{ c.name }}</router-link>
        </div>
      </div>
    </section>

    <!-- ── FAQ ───────────────────────────────────── -->
    <section class="section" style="background:var(--bg-soft)">
      <div class="container" style="max-width:720px">
        <div class="text-center mb-8">
          <div class="section-tag">FAQ</div>
          <h2>Частые вопросы</h2>
        </div>
        <div class="faq-list">
          <div v-for="(f,i) in faqs" :key="i" class="faq-item" :class="{open:openFaq===i}">
            <button class="faq-q" @click="openFaq=openFaq===i?null:i">
              <span>{{ f.q }}</span>
              <span class="faq-toggle">{{ openFaq===i ? '−' : '+' }}</span>
            </button>
            <transition name="slide-up">
              <div v-if="openFaq===i" class="faq-a">{{ f.a }}</div>
            </transition>
          </div>
        </div>
      </div>
    </section>

    <!-- ── CTA ───────────────────────────────────── -->
    <section class="cta-section">
      <div class="container cta-inner">
        <div>
          <h2 class="cta-title">Готовы запустить свой проект?</h2>
          <p class="cta-sub">Присоединяйтесь к тысячам создателей, которые уже нашли поддержку</p>
        </div>
        <router-link to="/register" class="btn btn-white btn-lg">Начать бесплатно →</router-link>
      </div>
    </section>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import ProjectCard from '@/components/project/ProjectCard.vue'
import { projectsApi, categoriesApi } from '@/services/api'

const loading   = ref(true)
const featured  = ref([])
const cats      = ref([])
const openFaq   = ref(null)
const activeTab = ref('Для создателей')

onMounted(async () => {
  try {
    const [p, c] = await Promise.all([projectsApi.getAll({ pageSize: 4 }), categoriesApi.getAll()])
    featured.value = p.data.items
    cats.value     = c.data
  } catch {}
  loading.value = false
})

function fmtM(n) { return new Intl.NumberFormat('ru-KZ',{maximumFractionDigits:0}).format(n||0)+' ₸' }
function truncate(s,n) { return s?.length>n ? s.slice(0,n)+'…' : s||'' }

const steps = [
  { icon:'📋', title:'Создайте проект', text:'Заполните страницу проекта: описание, цель, вознаграждения. Занимает около 15 минут.' },
  { icon:'✅', title:'Пройдите модерацию', text:'Наша команда проверит проект в течение 1–3 рабочих дней и опубликует его.' },
  { icon:'📣', title:'Продвигайте проект', text:'Делитесь в соцсетях, рассказывайте своей аудитории. Чем больше людей узнает — тем лучше.' },
  { icon:'💰', title:'Получите финансирование', text:'После успешного сбора деньги поступят на ваш счёт. Можно начинать реализацию!' },
]

const whyData = {
  'Для создателей': [
    { icon:'💸', title:'Без кредитов', text:'Привлекайте финансирование напрямую от сообщества, без банков и сложных условий.' },
    { icon:'📣', title:'Проверка спроса', text:'Получайте реальную обратную связь от аудитории до начала производства.' },
    { icon:'🤝', title:'Сообщество', text:'Превращайте спонсоров в долгосрочных сторонников и первых клиентов.' },
    { icon:'🛡️', title:'Минимум риска', text:'Платите только при успехе. Нет сборов — нет комиссии платформы.' },
  ],
  'Для спонсоров': [
    { icon:'🔍', title:'Прозрачность', text:'Полная история транзакций. Видите каждый тенге своей поддержки.' },
    { icon:'🌍', title:'Разнообразие', text:'Сотни проектов в 19 категориях — технологии, искусство, еда и многое другое.' },
    { icon:'🔒', title:'Безопасность', text:'Если цель не достигнута — средства возвращаются на ваш баланс автоматически.' },
    { icon:'🎁', title:'Вознаграждения', text:'Получайте эксклюзивные продукты и бонусы за поддержку первыми.' },
  ],
}
const whyItems = computed(() => whyData[activeTab.value])

const faqs = [
  { q:'Как создать проект?', a:'Зарегистрируйтесь, нажмите «Создать проект» и заполните 5-шаговую форму. Это займёт около 15 минут. После проверки модератором проект выходит в эфир.' },
  { q:'Какие комиссии у платформы?', a:'Мы берём небольшую комиссию только с успешно завершённых проектов. Подробнее в разделе «Тарифы».' },
  { q:'Что если проект не наберёт нужную сумму?', a:'Средства спонсоров возвращаются на их баланс. Вы можете пересмотреть проект и запустить заново.' },
  { q:'Как поддержать проект?', a:'Выберите проект → нажмите «Поддержать» → выберите вознаграждение и сумму → подтвердите оплату с баланса.' },
  { q:'Можно ли запустить проект из другого города?', a:'Да! Платформа работает по всему Казахстану. Укажите свой город при создании проекта.' },
]
</script>

<style scoped>
/* ── Hero ────────────────────────────────────── */
.hero {
  background: var(--bg);
  border-bottom: 1px solid var(--border);
  padding: 80px 0 72px;
}
.hero-inner {
  display: grid; grid-template-columns: 1fr 1fr;
  gap: 64px; align-items: center;
}
.hero-tag {
  display: inline-block; font-size: 12px; font-weight: 700;
  letter-spacing: .07em; text-transform: uppercase;
  color: var(--blue); background: var(--blue-light);
  padding: 5px 14px; border-radius: var(--r-full);
  margin-bottom: 20px;
}
.hero-title {
  font-size: clamp(2.2rem,4.5vw,3.4rem);
  font-weight: 800; line-height: 1.1; margin-bottom: 20px;
}
.hero-blue { color: var(--blue); }
.hero-desc {
  font-size: 17px; color: var(--text-mid); line-height: 1.7;
  max-width: 440px; margin-bottom: 36px;
}
.hero-btns { display: flex; gap: 12px; flex-wrap: wrap; margin-bottom: 48px; }

.hero-stats { display: flex; align-items: center; gap: 28px; }
.hs-item { }
.hs-num {
  font-family: 'Bricolage Grotesque', sans-serif;
  font-size: 1.7rem; font-weight: 800; color: var(--text);
}
.hs-lbl { font-size: 12px; color: var(--text-muted); margin-top: 3px; }
.hs-sep { width: 1px; height: 36px; background: var(--border); }

/* Hero cards */
.hc-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 14px; }
.hc-card {
  background: var(--bg); border: 1px solid var(--border);
  border-radius: var(--r-md); padding: 18px; cursor: pointer;
  transition: all .2s ease; box-shadow: var(--shadow-xs);
}
.hc-card:hover { border-color: var(--blue); box-shadow: var(--shadow-md); transform: translateY(-2px); }
.hc-top { display: flex; justify-content: space-between; align-items: center; margin-bottom: 10px; }
.hc-cat { font-size: 11px; font-weight: 700; text-transform: uppercase; letter-spacing: .04em; color: var(--text-muted); }
.hc-pct { font-family: 'Bricolage Grotesque', sans-serif; font-size: 15px; font-weight: 800; }
.hc-title { font-size: 13px; font-weight: 600; color: var(--text); line-height: 1.35; margin-bottom: 12px; }
.hc-amount { font-size: 12px; font-weight: 600; color: var(--blue); margin-top: 8px; }

/* ── Section header ─────────────────────────── */
.sec-head {
  display: flex; justify-content: space-between;
  align-items: flex-end; margin-bottom: 32px;
}
.sec-head h2 { margin-top: 8px; }

/* ── How it works ───────────────────────────── */
.how-grid {
  display: grid; grid-template-columns: repeat(4,1fr); gap: 20px;
}
.how-card {
  background: var(--bg); border: 1px solid var(--border);
  border-radius: var(--r-lg); padding: 28px; position: relative;
  transition: all .2s ease;
}
.how-card:hover { border-color: var(--blue); box-shadow: var(--shadow-sm); transform: translateY(-2px); }
.how-num {
  font-family: 'Bricolage Grotesque', sans-serif;
  font-size: 2.8rem; font-weight: 800; color: var(--blue-light);
  line-height: 1; position: absolute; top: 16px; right: 20px;
  letter-spacing: -.04em;
}
.how-icon { font-size: 28px; margin-bottom: 14px; }
.how-title { font-size: 1rem; font-weight: 700; color: var(--text); }

/* ── Why ────────────────────────────────────── */
.why-tabs { display: flex; gap: 8px; justify-content: center; margin-bottom: 32px; }
.why-tab {
  padding: 9px 24px; border-radius: var(--r-full);
  border: 1.5px solid var(--border); background: var(--bg);
  font-size: 14px; font-weight: 600; cursor: pointer;
  color: var(--text-mid); transition: all .18s;
  font-family: 'Inter', sans-serif;
}
.why-tab.active { background: var(--blue); color: #fff; border-color: var(--blue); box-shadow: var(--shadow-blue); }
.why-tab:not(.active):hover { border-color: var(--blue); color: var(--blue); }
.why-icon { font-size: 28px; }

/* ── Categories ─────────────────────────────── */
.cats-grid { display: flex; flex-wrap: wrap; gap: 10px; justify-content: center; }
.cat-pill {
  padding: 9px 18px; border-radius: var(--r-full);
  border: 1.5px solid var(--border); background: var(--bg);
  font-size: 13px; font-weight: 500; color: var(--text-mid);
  text-decoration: none; transition: all .18s;
}
.cat-pill:hover { border-color: var(--blue); color: var(--blue); background: var(--blue-light); text-decoration: none; }

/* ── FAQ ────────────────────────────────────── */
.faq-list { display: flex; flex-direction: column; gap: 0; }
.faq-item { border-bottom: 1px solid var(--border); }
.faq-item:first-child { border-top: 1px solid var(--border); }
.faq-q {
  width: 100%; display: flex; justify-content: space-between;
  align-items: center; padding: 20px 0; font-size: 15px;
  font-weight: 600; color: var(--text); cursor: pointer;
  background: none; border: none; text-align: left;
  gap: 16px; font-family: 'Inter', sans-serif;
  transition: color .15s;
}
.faq-q:hover { color: var(--blue); }
.faq-item.open .faq-q { color: var(--blue); }
.faq-toggle { font-size: 22px; color: var(--blue); flex-shrink: 0; font-weight: 400; }
.faq-a {
  padding-bottom: 20px; font-size: 14px;
  color: var(--text-mid); line-height: 1.7;
}

/* ── CTA ────────────────────────────────────── */
.cta-section {
  background: var(--blue);
  padding: 64px 0;
}
.cta-inner {
  display: flex; justify-content: space-between;
  align-items: center; gap: 32px;
}
.cta-title {
  font-family: 'Bricolage Grotesque', sans-serif;
  font-size: 2rem; font-weight: 800; color: #fff; margin-bottom: 8px;
}
.cta-sub { font-size: 15px; color: rgba(255,255,255,.75); }

@media (max-width: 900px) {
  .hero-inner { grid-template-columns: 1fr; }
  .hero-cards { display: none; }
  .how-grid   { grid-template-columns: 1fr 1fr; }
  .cta-inner  { flex-direction: column; text-align: center; }
}
@media (max-width: 600px) {
  .how-grid { grid-template-columns: 1fr; }
  .sec-head { flex-direction: column; gap: 14px; align-items: flex-start; }
}
</style>
