<template>
  <router-link :to="`/projects/${project.id}`" class="proj-card">
    <div class="proj-img">
      <img v-if="project.coverImageUrl" :src="project.coverImageUrl" :alt="project.title" />
      <div v-else class="proj-img-placeholder">
        <span>{{ project.categoryIcon || '💡' }}</span>
      </div>
      <div class="proj-img-overlay">
        <span class="badge badge-primary">{{ project.categoryName }}</span>
      </div>
      <div v-if="project.daysLeft <= 5 && project.daysLeft > 0" class="proj-urgent">
        🔥 Осталось {{ project.daysLeft }} дн.
      </div>
    </div>

    <div class="proj-body">
      <div class="proj-location">📍 {{ project.city || 'Казахстан' }}</div>
      <h3 class="proj-title">{{ project.title }}</h3>
      <p class="proj-desc">{{ truncate(project.shortDesc, 80) }}</p>

      <div class="proj-progress">
        <div class="progress progress-sm">
          <div class="progress-fill" :class="{ green: pct >= 100 }" :style="{ width: Math.min(100,pct) + '%' }" />
        </div>
      </div>

      <div class="proj-stats">
        <div class="proj-stat">
          <div class="ps-val">{{ fmtMoney(project.collectedAmount) }}</div>
          <div class="ps-lbl">из {{ fmtMoney(project.goalAmount) }}</div>
        </div>
        <div class="proj-stat-right">
          <div class="ps-pct" :class="{ green: pct >= 100 }">{{ Math.min(100,pct) }}%</div>
          <div class="ps-days">
            <template v-if="project.daysLeft > 0">{{ project.daysLeft }} дн. осталось</template>
            <template v-else><span style="color:var(--green)">Завершён</span></template>
          </div>
        </div>
      </div>

      <div class="proj-footer">
        <div class="proj-backers">
          <span class="backers-num">{{ project.backersCount }}</span> спонсоров
        </div>
        <div class="proj-arrow">Подробнее →</div>
      </div>
    </div>
  </router-link>
</template>

<script setup>
import { computed } from 'vue'
const props = defineProps({ project: { type: Object, required: true } })
const pct = computed(() =>
  props.project.goalAmount > 0
    ? Math.round(props.project.collectedAmount * 100 / props.project.goalAmount)
    : 0
)
function fmtMoney(n) {
  return new Intl.NumberFormat('ru-KZ', { maximumFractionDigits: 0 }).format(n || 0) + ' ₸'
}
function truncate(str, len) {
  return str?.length > len ? str.slice(0, len) + '…' : str || ''
}
</script>

<style scoped>
.proj-card {
  display: flex; flex-direction: column;
  background: var(--bg); border: 1px solid var(--border);
  border-radius: var(--r-lg); overflow: hidden;
  text-decoration: none; color: inherit;
  transition: all .22s ease;
  box-shadow: var(--shadow-xs);
}
.proj-card:hover {
  border-color: var(--blue);
  box-shadow: var(--shadow-md), 0 0 0 1px var(--blue-mid);
  transform: translateY(-4px);
  text-decoration: none;
}

.proj-img {
  position: relative; height: 188px; overflow: hidden;
  background: var(--bg-subtle); flex-shrink: 0;
}
.proj-img img {
  width: 100%; height: 100%; object-fit: cover;
  transition: transform .4s ease;
}
.proj-card:hover .proj-img img { transform: scale(1.04); }
.proj-img-placeholder {
  width: 100%; height: 100%;
  display: flex; align-items: center; justify-content: center;
  font-size: 52px;
  background: linear-gradient(135deg, var(--blue-light), var(--bg-subtle));
}
.proj-img-overlay { position: absolute; top: 12px; left: 12px; }
.proj-urgent {
  position: absolute; top: 12px; right: 12px;
  background: #FFF3E0; color: var(--warning);
  border: 1px solid #FFD58A;
  font-size: 11px; font-weight: 700; padding: 3px 10px;
  border-radius: var(--r-full);
}

.proj-body { padding: 18px 20px 20px; display: flex; flex-direction: column; flex: 1; }
.proj-location { font-size: 12px; color: var(--text-muted); margin-bottom: 6px; }
.proj-title {
  font-family: 'Bricolage Grotesque', sans-serif;
  font-size: 1.05rem; font-weight: 700; color: var(--text);
  line-height: 1.3; margin-bottom: 8px;
  display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden;
}
.proj-desc {
  font-size: 13px; color: var(--text-muted);
  line-height: 1.55; margin-bottom: 16px; flex: 1;
}

.proj-progress { margin-bottom: 12px; }

.proj-stats {
  display: flex; justify-content: space-between;
  align-items: flex-end; margin-bottom: 16px;
}
.ps-val { font-size: 14px; font-weight: 700; color: var(--blue); }
.ps-lbl { font-size: 11px; color: var(--text-muted); margin-top: 2px; }
.proj-stat-right { text-align: right; }
.ps-pct { font-family: 'Bricolage Grotesque', sans-serif; font-size: 1.5rem; font-weight: 800; color: var(--text); }
.ps-pct.green { color: var(--green); }
.ps-days { font-size: 11px; color: var(--text-muted); margin-top: 2px; }

.proj-footer {
  display: flex; justify-content: space-between; align-items: center;
  padding-top: 14px; border-top: 1px solid var(--border);
}
.proj-backers { font-size: 12px; color: var(--text-muted); }
.backers-num { font-weight: 700; color: var(--text); }
.proj-arrow {
  font-size: 12px; font-weight: 600; color: var(--blue);
  transition: transform .2s;
}
.proj-card:hover .proj-arrow { transform: translateX(4px); }
</style>
